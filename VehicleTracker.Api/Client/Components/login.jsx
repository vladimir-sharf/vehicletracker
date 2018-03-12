import React from 'react';

const Login = ({ store }) =>
  <div>
    <button
      onClick={() => store.auth()}>
      Log in
    </button>
  </div>;

export default Login;