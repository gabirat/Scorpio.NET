/* eslint-disable */

export default class Validators {
  static required = value => (value ? undefined : "Field is required");
  static number = value => (isNaN(value) ? "Value must be a number" : undefined);
  static minNumber = min => value => (isNaN(value) || value >= min ? undefined : `Should be greater than ${min}`);
  static maxNumber = max => value => (isNaN(value) || value <= max ? undefined : `Should be less than ${max}`);
  static email = email => (this._validateEmail(email) ? undefined : "Invalid E-Mail");
  static ipV4 = value => (this._evaluateRegex("^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}$", value) ? undefined : "Invalid IP address");
  static regex = (regex, message) => value => (this._evaluateRegex(regex, value) ? undefined : message);

  static compose = (...validators) => value => validators.reduce((error, validator) => error || validator(value), undefined);

  static _evaluateRegex(regex, value) {
    if (regex && value) return new RegExp(regex).test(value);
    return false;
  }

  static _validateEmail(email) {
    return this._evaluateRegex(
      /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
      email
    );
  }
}
