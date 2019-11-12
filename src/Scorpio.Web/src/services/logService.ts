/* eslint-disable no-console */

enum LogLevelEnum {
  trace = 1,
  debug = 3,
  info = 5,
  warn = 7,
  error = 9
}

class LogService {
  static trace(...args: any[]) {
    if (!this._shouldLog(LogLevelEnum.trace)) return;
    console.debug("[TRC]", ...args);
  }

  static debug(...args: any[]) {
    if (!this._shouldLog(LogLevelEnum.debug)) return;
    console.debug("[DBG]", ...args);
  }

  static info(...args: any[]) {
    if (!this._shouldLog(LogLevelEnum.info)) return;
    console.log("[INF]", ...args);
  }

  static warn(...args: any[]) {
    if (!this._shouldLog(LogLevelEnum.warn)) return;
    console.warn("[WRN]", ...args);
  }

  static error(...args: any[]) {
    if (!this._shouldLog(LogLevelEnum.error)) return;
    console.error("[ERR]", ...args);
  }

  static _shouldLog(severity: LogLevelEnum) {
    const logLevel = LogLevelEnum.trace;
    return severity >= logLevel;
  }
}
/* eslint-enable no-console */

export default LogService;
