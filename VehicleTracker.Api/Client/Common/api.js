export default class Api {
  constructor(http) {
    this.http = http;
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

  subscribeVehicles() {
    return this.http.subscribeHub('/vehicleHub');
  }
}