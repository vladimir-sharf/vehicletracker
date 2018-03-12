import { HubConnection } from '@aspnet/signalr';

export default class Api {
  constructor(http, userManager) {
    this.http = http;
    this.userManager = userManager;
  }

  loadCustomers() {
    return this.request('get', '/customers');
  }

  loadVehicles(query) {
    return this.request('get', `/vehicles?${query}`);
  }

  async request(method, url, body) {
    try {
      return await this.http[method](url, body);
    } catch (err) {
      throw err;
    }
  }

  async subscribeVehicles() {
    var user = await this.userManager.getUser();
    return new HubConnection('/vehicleHub', {
      accessTokenFactory: () => user.access_token
    });
  }
}