export const removeNulls = (obj) =>
  Object.keys(obj)
    .filter(k => obj[k] !== null && obj[k] !== undefined)
    .reduce((map, k) => {
      map[k] = obj[k];
      return map;
    }, {});

export const toQueryString = (obj) =>
  Object.keys(obj)
    .map((v) => `${v}=${obj[v]}`)
    .join('&');