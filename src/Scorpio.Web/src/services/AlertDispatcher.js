import Alert from "react-s-alert";

export default class AlertDispatcher {
  static dispatchSuccess(msg) {
    if (msg) {
      AlertDispatcher.dispatch({ type: "success", text: msg });
    }
  }

  static dispatchError(msg) {
    if (msg) {
      AlertDispatcher.dispatch({ type: "error", text: msg });
    }
  }

  static dispatch(alerts) {
    if (!alerts) return;

    // multiple alerts - go one by one
    if (Array.isArray(alerts)) {
      alerts.forEach(alert => {
        this._doDispatch(alert);
      });
      return;
    }

    this._doDispatch(alerts);
  }

  static _doDispatch(alert) {
    if (!alert || !alert.text || !alert.type) return;

    switch (alert.type) {
      case "success":
        Alert.success(alert.text, this._getAlertData());
        break;

      case "info":
        Alert.info(alert.text, this._getAlertData());
        break;

      case "warn":
        Alert.warning(alert.text, this._getAlertData());
        break;

      case "error":
        Alert.error(alert.text, this._getAlertData());
        break;

      default:
        Alert.info(alert.text, this._getAlertData());
        break;
    }
  }

  static _getAlertData() {
    return {
      offset: 0,
      effect: "slide",
      html: false,
      position: "top-right"
    };
  }
}
