import React from 'react';
import { observer } from 'mobx-react';
import styles from './dropdown.css';

function substituteNull(value) {
    return value === null ? "" : value;
}

@observer
export default class Dropdown extends React.Component
{
    onChange(e) {
        const { onChange } = this.props;
        if (!onChange)
          return;
        for (let node of e.target.children) {
          if (node.value === e.target.value) {
            const id = node.getAttribute('data-id');
            onChange({id, value : e.target.value});
            return;
          }
        }    
      }
    
    render() {
        const { selected, items, disabled, onChange } = this.props;
        return <select onChange={this.onChange.bind(this)} value={selected === null ? "" : selected}>
            {items.map((item) => <option key={item.id} data-id={item.id === null ? "" : item.id}>{item.name}</option>)}
        </select >;
    }
}