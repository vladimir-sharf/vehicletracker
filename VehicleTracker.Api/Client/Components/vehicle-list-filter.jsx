import React from 'react';
import { observer } from 'mobx-react';
import styles from './vehicle-list-filter.css';
import CustomerDropDown from './customer-dropdown';
import StatusDropDown from './status-dropdown';
import VehicleList from './vehicle-list';

@observer
export default class VehicleListFilter extends React.Component {
  componentWillMount() {
    this.load(this.props);
  }

  componentWillUpdate(nextProps) {
    this.load(nextProps);
  }

  componentWillUnmount() {
    vehicleStore.poll();
  }

  load(props) {
    const { location } = props;
    const { customerStore, vehicleStore, filtersStore } = props.store;
    filtersStore.setFromQueryString(location.search);
    customerStore.load();
    vehicleStore.poll();
  }

  onCustomerFilterChange(obj) {
    const { filtersStore } = this.props.store;
    filtersStore.customer = obj.id;
    this.updateUrl();
  }

  onStatusFilterChange(obj) {
    const { filtersStore } = this.props.store;
    filtersStore.status = obj.id === null ? obj.id : Number(obj.id);
    this.updateUrl();
  }

  updateUrl() {
    const { location, history } = this.props;
    const { filtersStore } = this.props.store;
    var query = filtersStore.updateQueryString(location.search);
    history.push(location.pathname + query);
  }

  render() {
    const { customerStore, vehicleStore, filtersStore } = this.props.store;
    return <div className={styles.root}>
      <div className={styles.filters}>
        <CustomerDropDown selected={filtersStore.customer} customers={customerStore.customers} onChange={this.onCustomerFilterChange.bind(this)} />
        <StatusDropDown selected={filtersStore.status} onChange={this.onStatusFilterChange.bind(this)} />
      </div>
      <VehicleList store={vehicleStore} />
    </div>;
  }
}