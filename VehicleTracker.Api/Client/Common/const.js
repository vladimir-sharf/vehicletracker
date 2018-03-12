export const VehicleStatus = {
  0: { id: 0, name: 'Connected' },
  1: { id: 1, name: 'Disconnected' },
  2: { id: 2, name: 'Unknown' }
};

export const VehicleStatusNames = Object.values(VehicleStatus)
  .reduce((map, v) => {
    map[v.name] = v.id;
    return map;
  }, {});

export const StatusValidSeconds = 60;