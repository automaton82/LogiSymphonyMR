## Logi Symphony in Mixed Reality

A sample app made using Unity for the Meta Quest 3 to render Logi Symphony in Mixed Reality.

The following repo was used for the WebView:
https://github.com/automaton82/SimpleUnity3DWebView

Helper scripts were added to dashboards to make the code cleaner:

```
window.revisToChart = (adapterName, chartType) => {
  const adapter = this.getAdapters().first(x => x.friendlyName == adapterName);
  const chartsInfo = new dundas.view.controls.ChartsInfo();
  const lineInfo = chartsInfo.getControlInfos().first(x => x.caption == chartType);
  const newAdapter = adapter.revisualize(chartsInfo, lineInfo, true);
  newAdapter.friendlyName = adapterName;
};

window.revisToTable = (adapterName) => {
  const adapter = this.getAdapters().first(x => x.friendlyName == adapterName);
  const dataGridInfo = new dundas.view.controls.DataGridInfo();
  const newAdapter = adapter.revisualize(dataGridInfo, dataGridInfo.getControlInfos()[0], true);
  newAdapter.friendlyName = adapterName;
};

window.setFilter = (productName) => {
  var viewParameter = this.control.getViewParameterByName("product");
  var filterValue = new dundas.data.MemberValue({
    hierarchyUniqueName: "Product",
    levelUniqueName: "C.Product", 
    uniqueName: productName + ".C.Product"
  });
  viewParameter.parameterValue.clearTokens();
  viewParameter.parameterValue.clearValues();
  viewParameter.parameterValue.values.push(filterValue);
  viewParameter.invalidateParameterValueLastModifiedTime();
  viewParameter.refreshAllAdapters(null, this);
};
```

Adjust as required, or move the code into the app itself. Tested with Logi Symphony 23.4 deployed in AWS.
