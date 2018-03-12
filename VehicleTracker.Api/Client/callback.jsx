import { Redirect } from 'react-router-dom';
import React from 'react';
import { observer } from 'mobx-react';

@observer
export default class Callback extends React.Component {
  componentWillMount(newProps) {
    const { store } = this.props;
    store.signinCallback();
  }

  render() {
    const { store } = this.props;
    return !store.loading ? <Redirect to="/" /> : <div />;
  }
}