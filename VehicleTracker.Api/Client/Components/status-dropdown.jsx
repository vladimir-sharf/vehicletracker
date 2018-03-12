import React from 'react';
import { observer } from 'mobx-react';
import styles from './status-dropdown.css';
import { VehicleStatus } from '../Common/const';

const statuses = [ { id: "", name: 'Filter by status...' }, ...Object.values(VehicleStatus)];

export default class StatusDropdown extends React.Component {
  onChange(e) {
    const { onChange } = this.props;
    if (!onChange)
      return;
    for (let node of e.target.children) {
      if (node.value === e.target.value) {
        const id = node.getAttribute('data-id');
        onChange({ id: id === "" ? null : id, value: e.target.value });
        return;
      }
    }
  }

  render() {
    const { selected, disabled, onChange } = this.props;
    return <select onChange={this.onChange.bind(this)} value={selected === null ? "" : selected}>
      {statuses.map((c) => <option key={c.id} data-id={c.id} value={c.id}>{c.name}</option>)}
    </select >;
  }
}