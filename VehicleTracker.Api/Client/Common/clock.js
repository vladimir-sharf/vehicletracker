import { Atom } from "mobx";

export default class Clock {
  atom;
  intervalHandler = null;
  currentDateTime;

  constructor() {
    this.atom = new Atom(
      "Clock",
      () => this.startTicking(),
      () => this.stopTicking()
    );
  }

  getTime() {
    if (this.atom.reportObserved()) {
      return this.currentDateTime;
    } else {
      return new Date();
    }
  }

  tick() {
    this.currentDateTime = new Date();
    this.atom.reportChanged();
  }

  startTicking() {
    this.tick();
    this.intervalHandler = setInterval(
      () => this.tick(),
      1000
    );
  }

  stopTicking() {
    clearInterval(this.intervalHandler);
    this.intervalHandler = null;
  }
}
