import styles from './Common/common.css';
import React from 'react';
import ReactDOM from 'react-dom';
import App from 'app';
import Callback from 'callback';
import Http from './Common/http';
import Api from './Common/api';
import RootStore from './Stores/root-store';
import { BrowserRouter as Router, Route } from 'react-router-dom';
import { UserManager, WebStorageStateStore } from 'oidc-client';
import Clock from './Common/clock';

var config = {
  authority: authParams.JsAuthority,
  client_id: authParams.ClientId,
  redirect_uri: authParams.RedirectUri,
  response_type: "id_token token",
  scope: `openid profile ${authParams.Scope}`,
  post_logout_redirect_uri: authParams.PostLogoutRedirectUri,
  userStore: new WebStorageStateStore({ store: window.localStorage })
};
var mgr = new UserManager(config);
var http = new Http(mgr);
var api = new Api(http);
var clock = new Clock();
var rootStore = new RootStore(api, mgr, clock);

ReactDOM.render(<Router>
  <div>
    <Route path="/callback" render={(props) => <Callback store={rootStore.authStore} />} />
    <Route path="/" store={rootStore.authStore} render={(props) => <App store={rootStore} {...props} />} />
  </div>
</Router>, document.getElementById('app'));
