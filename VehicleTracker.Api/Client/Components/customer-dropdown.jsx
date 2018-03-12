import React from 'react';
import { observer } from 'mobx-react';
import styles from './customer-dropdown.css';

@observer
export default class CustomerDropdown extends React.Component {
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
    const { selected, customers, disabled, onChange } = this.props;
    return <select onChange={this.onChange.bind(this)} value={selected || ""}>
      <option key=''>Filter by customer...</option>
      {customers.map((c) => <option key={c.id} data-id={c.id} value={c.id}>{c.name}</option>)}
    </select >;
  }
}