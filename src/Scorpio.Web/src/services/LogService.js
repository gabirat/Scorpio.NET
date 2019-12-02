/* eslint-disable no-console */
class LogService {
  static trace(...args) {
    if (!this._shouldLog("trace")) return;
    console.debug("[TRC]", ...args);
  }

  static debug(...args) {
    if (!this._shouldLog("debug")) return;
    console.debug("[DBG]", ...args);
  }

  static info(...args) {
    if (!this._shouldLog("info")) return;
    console.log("[INF]", ...args);
  }

  static warn(...args) {
    if (!this._shouldLog("warn")) return;
    console.warn("[WRN]", ...args);
  }

  static error(...args) {
    if (!this._shouldLog("error")) return;
    console.error("[ERR]", ...args);
  }

  static _shouldLog(severity) {
    const logLevel = "trace";
    return logLevelEnum[severity] >= logLevelEnum[logLevel];
  }
}
/* eslint-enable no-console */

const logLevelEnum = Object.freeze({
  trace: 1,
  debug: 3,
  info: 5,
  warn: 7,
  error: 9
});

export default LogService;
