export const updateFromJson = (storeArray, json, TModel, ModelParams) => {
  const storeHash = storeArray.reduce(addMap, {});

  json.forEach(c => {
    var item = storeHash[c.id];
    if (!item) {
      item = new TModel(c.id, ModelParams);
      storeArray.push(item);
    }
    item.updateFromJson(c);
  });

  const jsonHash = json.reduce(addMap, {});

  storeArray
    .filter(c => !jsonHash[c.id])
    .forEach(c => removeFromStore(storeArray, c));
};

function addMap(map, c) {
  map[c.id] = c;
  return map;
}

export const removeFromStore = (storeArray, model) => 
  storeArray.splice(storeArray.indexOf(model), 1);