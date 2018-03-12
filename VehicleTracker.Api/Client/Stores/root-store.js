import CustomerStore from './customer-store';
import VehicleStore from './vehicle-store';
import FiltersStore from './filters-store';
import AuthStore from './auth-store';

export default class RootStore {
    constructor(api, userManager, clock) {
        this.customerStore = new CustomerStore(api);
        this.vehicleStore = new VehicleStore(this, api, clock);
        this.authStore = new AuthStore(userManager);
        this.filtersStore = new FiltersStore();
    }
}