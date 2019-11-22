export const mapDictionaryToDropdownOptions = (dictionaries, name) => {
  if (!name || !dictionaries || !dictionaries.length === 0) return [];

  const properTypeDict = dictionaries.find(x => x.name.toLowerCase() === name.toLowerCase() && typeof x !== undefined);

  if (!properTypeDict || properTypeDict.length === 0) {
    return [];
  } else {
    return properTypeDict.values
      .filter(x => x.isActive)
      .map(x => {
        return {
          key: x.id === 0 ? x.internalValue : x.id,
          value: x.id === 0 ? x.internalValue : x.id,
          text: x.displayValue
        };
      });
  }
};

export const mapDictionaryToValue = (dictionaries, name, id) => {
  if (!name || !dictionaries || !dictionaries.length === 0) return [];

  const properTypeDict = dictionaries.find(x => x.name.toLowerCase() === name.toLowerCase() && typeof x !== undefined);

  if (!properTypeDict || properTypeDict.length === 0) return "";
  else {
    const dict = properTypeDict.values.find(x => x.id === id);
    if (dict && dict.displayValue) return dict.displayValue;
    else return "";
  }
};
