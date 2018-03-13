import { observable, computed, action, runInAction } from 'mobx';
import { updateFromJson, removeFromStore } from './store-utils';
import { removeNulls, toQueryString } from '../Common/utils';
import { VehicleStatus, VehicleStatusNames, StatusValidSeconds } from '../Common/const';
import moment from 'moment';

export default class VehicleStore {
  @observable vehicles = [];
  @observable loading = false;

  @computed
  get filtered() {
    const status = this.rootStore.filtersStore.status;
    return status === null ? this.vehicles : this.vehicles.filter(v => v.status === status);
  }

  constructor(rootStore, api, clock) {
    this.rootStore = rootStore;
    this.api = api;
    this.clock = clock;
  }

  @action
  stop() {
    if (this.connection)
      this.connection.stop();

    this.connection = null;
  }

  @action
  async poll() {
    if (this.connection)
      this.connection.stop();

    this.load();

    this.connection = await this.api.subscribeVehicles();
    runInAction(() => {
      this.connection.on("send", this.updateVehicleStatus.bind(this));
      this.connection.start();
    })
  }

  @action
  updateVehicleStatus(data) {
    const vehicle = this.vehicles.filter(v => v.id === data.id)[0];
    if (vehicle)
      vehicle.updateFromMessage(data);
  }

  @action
  async load() {
    const filtersStore = this.rootStore.filtersStore;
    this.loading = true;
    const json = await this.api.loadVehicles(toQueryString(removeNulls({
      customerId: filtersStore.customer,
      // status: filtersStore.status
    })));
    runInAction(() => {
      updateFromJson(this.vehicles, json, Vehicle, this.clock);
      this.loading = false;
    });
  }
}

class Vehicle {
  id = null;
  @observable regNr = null;
  @observable _status = null;
  @observable customerId = null;
  @observable customerName = null;
  @observable timeUtc = null;

  constructor(id, clock) {
    this.id = id;
    this.clock = clock;
  }

  @computed
  get secondsAgo() {
    const time = this.clock.getTime();
    const result = Math.floor((time.getTime() - (new Date(this.timeUtc)).getTime()) / 1000);
    return result > 0 ? result : 0;
  }

  @computed
  get status() {
    if (this.secondsAgo > StatusValidSeconds) {
      return VehicleStatusNames.Unknown;
    }
    return this._status;
  }

  @computed
  get statusName() {
    return VehicleStatus[this.status].name;
  }

  @computed
  get statusHistory() {
    return VehicleStatus[this._status].name;
  }

  @computed
  get updatedTime() {
    return moment(this.timeUtc).format("HH:mm:ss");
  }

  @action
  updateFromJson(json) {
    this.regNr = json.regNr;
    this._status = json.status;
    this.customerId = json.customerId;
    this.customerName = json.customerName;
    this.timeUtc = json.timeUtc;
  }

  @action
  updateFromMessage(data) {
    this._status = data.status;
    this.timeUtc = data.timeUtc;
  }
}