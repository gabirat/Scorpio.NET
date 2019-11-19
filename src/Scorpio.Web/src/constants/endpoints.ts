export default class Endpoints {
  public static backendUrl: string | undefined = process.env.REACT_APP_BACKEND_URL;

  public static Hub = Endpoints.backendUrl + "/hub/main";

  public static GetConfigs: string = Endpoints.backendUrl + "/api/Configuration";
}
