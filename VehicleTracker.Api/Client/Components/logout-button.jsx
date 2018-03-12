import React from 'react';
import { withRouter } from 'react-router-dom';

const LogoutButton = withRouter(
  ({ store, history }) =>
    store.isAuthentificated ? (
      <div>
        <button
          onClick={() => {
            store.signout();
            history.push("/");
          }}>
          Sign out
        </button>
      </div>
    ) : <div />
);

export default LogoutButton;