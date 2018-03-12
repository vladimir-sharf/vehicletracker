import React from 'react';
import { observer } from 'mobx-react';
import styles from './vehicle-list.css';

@observer
export default class VehicleList extends React.Component
{
  render() {
    const { filtered } = this.props.store;
    return <div className={styles.root}>
        <VehicleListHeader/>
        {filtered.map((v, i) => <Vehicle className={i % 2 == 0 ? styles.even : styles.odd} key={v.id} vehicle={v}/>)}
    </div>;
  }
}

const VehicleListHeader = () => <div className={styles.header}>
        <div>Customer</div>
        <div>VIN</div>
        <div>Reg. Nr.</div>
        <div>Status</div>
        <div>Updated</div>
        <div>Latest status</div>
    </div>;

@observer
class Vehicle extends React.Component {
    render() {
        const { vehicle, className } = this.props;
        return <div className={className}>
            <div>{vehicle.customerName}</div>
            <div>{vehicle.id}</div>
            <div>{vehicle.regNr}</div>
            <div><VehicleStatus id={vehicle.status} name={vehicle.statusName}/></div>
            <div>{vehicle.updatedTime}</div>
            <div>{vehicle.statusHistory}</div>
        </div>;
    }
}

class VehicleStatus extends React.Component {
  dotClassName(id) {
    switch (id) {
      case 0:
        return `${styles.dot} ${styles.connected}`;
      case 1:
        return `${styles.dot} ${styles.disconnected}`;
      default:
        return styles.dot;
    }
  }

  render() {
    const { id, name } = this.props;
    return <div className={styles.vehicleStatus}>
      <div className={this.dotClassName(id)} />
      {name && <div>{name}</div>}
    </div>;
  }
}