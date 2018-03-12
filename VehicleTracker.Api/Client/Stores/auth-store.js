import { observable, action, computed, runInAction } from 'mobx';

export default class AuthStore {
  @observable
  isAuthentificated = false;

  @observable
  loading = false;

  @observable
  user = null;

  constructor(userManager) {
    this.userManager = userManager;
  }

  @action
  auth(credentials) {
    this.userManager.signinRedirect();
  }

  @action
  signinCallback() {
    this.loading = true;
    this.userManager.signinRedirectCallback()
      .then(() => runInAction(() => {
        this.isAuthentificated = true;
        this.loading = false;
      }))
      .catch(e => runInAction(() => {
        this.loading = false;
        console.error(e);
      }));
  }

  @action
  async load() {
    try {
      const user = await this.userManager.getUser();
      runInAction(() => {
        this.user = user;
        this.isAuthentificated = !!user;
      });
    }
    catch (e) {
      console.error(e);
      runInAction(() => {
        this.isAuthentificated = false;
        this.user = null;
      });
    }
  }

  @action
  signout() {
    this.userManager.signoutRedirect();
  }
}