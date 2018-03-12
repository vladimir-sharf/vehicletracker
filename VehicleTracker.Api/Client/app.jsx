import React from 'react';
import { observer } from 'mobx-react';
import VehicleListFilter from './Components/vehicle-list-filter';
import Login from './Components/login';
import LogoutButton from './Components/logout-button';
import styles from './app.css';

@observer
export default class App extends React.Component {
  componentWillMount() {
    this.load(this.props);
  }

  componentWillUpdate(nextProps) {
    this.load(nextProps);
  }

  load(props) {
    const { authStore } = props.store;
    authStore.load();
  }

  render() {
    const authStore = this.props.store.authStore;
    return authStore.isAuthentificated ? <div className={styles.root}>
      <div className={styles.header}>
        <h1>Vehicle Tracker</h1>
        <LogoutButton store={authStore} />
      </div>
      <VehicleListFilter {...this.props} />
    </div> : <Login store={authStore} />;
  }
}