export enum EMethod {
  GET = "GET",
  POST = "POST",
  PUT = "PUT",
  DELETE = "DELETE"
}

export default async function genericApi(endpoint: string, method: EMethod, body: any) {
  return await fetch(endpoint, getFetchParam(method, body))
    .then((response: Response) => handleResponse(response))
    .catch((error: Error) => console.log("Caught error: ", error));
}

function handleResponse(response: Response) {
  if (response) {
    return response.json().then((data: any) => {
      return {
        status: response.status,
        body: data
      };
    });
  }
}

function getFetchParam(method: EMethod, body: any): RequestInit {
  return {
    method: method,
    headers: {
      "Content-type": "application/json; charset=UTF-8",
      Accept: "application/json"
    },
    body: (method === EMethod.POST || method === EMethod.PUT) && body ? JSON.stringify(body) : undefined
  };
}
