import { observable, action, computed } from 'mobx';
import queryString from 'query-string'
import { removeNulls, toQueryString } from '../Common/utils';

export default class FiltersStore {
  @observable customer = null;
  @observable status = null;

  @action
  setFromQueryString(search) {
    const qs = queryString.parse(search);
    this.customer = qs.customer || null;
    this.status = 'status' in qs ? Number(qs.status) : null;
  }

  updateQueryString(search) {
    const qs = Object.assign({}, queryString.parse(search), { customer: this.customer, status: this.status });
    const result = toQueryString(removeNulls(qs));
    return result && ('?' + result);
  }
}