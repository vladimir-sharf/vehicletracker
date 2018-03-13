import fetch from 'isomorphic-fetch';
import { HubConnection } from '@aspnet/signalr';

export default class Http {
  constructor(userManager) {
    this.userManager = userManager;
  }

  get(url) {
    return this._request(url, 'GET');
  }

  post(url, body) {
    return this._request(url, 'POST', body);
  }

  put(url, body) {
    return this._request(url, 'PUT', body);
  }

  delete(url) {
    return this._request(url, 'DELETE');
  }

  async _request(url, method, body) {
    const user = this.userManager && await this.userManager.getUser();

    const params = {
      method: method,
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json; charset=UTF-8',
      }
    };

    if (user) {
      params.headers['Authorization'] = `Bearer ${user.access_token}`;
    }

    if (body)
      params.body = JSON.stringify(body);

    let response;
    try {
      response = await fetch(url, params);
    } catch (err) {
      // Workaround for a strange EDGE bug. Normally fetch doesn't throw on error codes
      if (err.name === 'TypeMismatchError')
        throw { edgeBug: true }
      throw err;
    }

    if (response.status == 401)
      return this.userManager.signinRedirect();

    const json = await response.json();
    if (response.ok)
      return json;

    throw { status: response.status, json: json };
  }

  async subscribeHub(url) {
    var user = await this.userManager.getUser();
    return new HubConnection(url, {
      accessTokenFactory: () => user.access_token
    });
  }
}