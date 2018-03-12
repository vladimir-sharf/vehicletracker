import { observable, computed, action, runInAction } from 'mobx';
import { updateFromJson, removeFromStore } from './store-utils';

export default class CustomerStore {
  @observable customers = [];
  @observable loading = false;

  constructor(api) {
    this._api = api;
  }

  @action
  async load(query) {
    this.loading = true;
    const json = await this._api.loadCustomers();
    runInAction(() => {
      updateFromJson(this.customers, json, Customer);
      this.loading = false;
    });
  }
}

class Customer {
  id = null;
  @observable name = null;
  @observable address = null;

  constructor(id = uuid.v4()) {
    this.id = id;
  }

  @action
  updateFromJson(json) {
    this.name = json.name;
    this.address = json.address;
  }
}