import { Component, Input, Output, OnInit } from '@angular/core';
import { NgForm, NgModel } from '@angular/forms';

import { TransactionsService } from '../../services/transactions.service';
import { UtilitiesService } from '../../services/utilities.service';
import { CatalogsService } from '../../services/catalogs.service';

import { DialogsTableComponent } from '../dialogs-table/dialogs-table.component';
import { DialogsAlertsComponent } from '../dialogs-alerts/dialogs-alerts.component';
import { MatDialog } from '@angular/material';
import { Angular5Csv } from 'angular5-csv/Angular5-csv';
import { TooltipPosition } from '@angular/material';

import * as html2canvas from 'html2canvas';
import { INTERNAL_BROWSER_DYNAMIC_PLATFORM_PROVIDERS } from '@angular/platform-browser-dynamic/src/platform_providers';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  working: boolean;
  chartWidth: any;
  chartHeight: any;

  totalMoney: number;
  totalTransaction: number;
  expandChartStores: boolean;
  expandChartPlatform: boolean;
  expandChartQuantity: boolean;
  expendChartStatus: boolean;

  dataStore: any = [];
  dataStoreDetails: any = [];
  dataPlatform: any = [];
  dataPlatformDetails: any = [];
  dataQuantity: any = [];
  dataQuantityDetails: any = [];
  dataStatus: any = [];
  dataStatusGraph: any = [];
  dataStatusDetail: any = [];

  stores = [];
  plataforms = [];
  storeId: any[];
  storeLast = [];
  platformId: any[];
  platfomLast = [];
  starDate: Date;
  endDate: Date;

  transansactions: any[];
  isModeDashboard: boolean;
  /******************************************************/
  /*  PROPIEDADES PARA LAS GRAFICAS EN NG2-CHARTS       */
  /******************************************************/

  // Grafica de barra para la tienda
  StoreBarChartOptions: any = {};
  StoreBarChartType = 'bar';
  StoreBarChartLegend = true;
  storeBarChartData: any[] = [
    { data: [0], label: 'E-COMMERCE' },
    { data: [0], label: 'VIAJES' },
    { data: [0], label: 'MESA DE REGALO' }
  ];

  // Grafica de barras para plataformas
  PlatformBarChartOptions: any = {};
  PlatformBarChartLegend = true;
  PlatformBarChartData: any[] = [
    { data: [0], label: 'Ecommerce' },
    { data: [0], label: 'Viajes' },
    { data: [0], label: 'Mesa de regalo' }
  ];

  // PUNTOS POR FECHA
  lineChartData: any[] = [{}];
  lineChartLabels: Array<any> = [];
  lineChartColors: Array<any> = [];
  lineChartOptions: any = {};

  lineExcelData: any = [];
  lineExcelQuantity: any = [];
  lineChartLegend = true;

  // DONA ERRORES
  doughnutChartLabels: string[] = ['Correcto', 'Pendiente', 'Alerta'];
  doughnutChartData: number[] = [0, 0, 0];
  doughnutOptions: any = [];
  doughnutColors: Array<any> = [];

  toolTipStore: string;
  toolTipPlatform: string;

  /** FIN PROPIEDADES NG2 */
  constructor(private transactionService: TransactionsService,
    private utilities: UtilitiesService,
    private catalogServices: CatalogsService,
    public dialog: MatDialog) {

    this.catalogServices.getCatalogsDashboard().subscribe(res => {

      this.plataforms = [];
      this.stores     = [];
      this.storeId    = [];
      this.platformId = [];

      if (res.isSuccess) {
        this.plataforms.push({ value: 0, viewValue: 'Todas las plataformas' });
        this.stores.push({ value: 0, viewValue: 'Todas las tiendas' });

        this.platformId.push(0);
        this.platfomLast.push(0);

        this.storeId.push(0);
        this.storeLast.push(0);

        for (let p of res.data.platforms) {
          this.plataforms.push({ value: p.id, viewValue: p.description });
          this.platformId.push(p.id);
          this.platfomLast.push(p.id);
        }

        for (let s of res.data.stores) {
          this.stores.push({ value: s.id, viewValue: s.description });
          this.storeId.push(s.id);
          this.storeLast.push(s.id);

        }

        this.setToolTipPlatform();
        this.setToolTipStore();

      }
    });

    // INICIALIZAR TODOS LAS VARIABLES UTILIZADAS
    this.working = true;
    this.storeId = [];
    this.platformId = [];
    this.endDate = new Date(Date.now());
    this.starDate = new Date();
    this.isModeDashboard = true;
    // NG2 FIN OPCIONES BARRA Platform
    this.starDate.setDate(this.endDate.getDate() - 7);
    this.endDate.setDate(this.endDate.getDate() + 30);

    this.initConfigChart();

  }

  ngOnInit() {
    this.onFilter();
  }
  // INICIALIZAR LA CONFIGURACIÓN DE LAS GRÁFICAS
  initConfigChart() {

    // NG2 INICIO opciones de barra Platform
    this.PlatformBarChartOptions = {
      maintainAspectRatio: false,
      scaleShowVerticalLines: true,
      responsive: true,
      legend: {
        position: 'bottom',
      },
      scales: {
        yAxes: [{
          ticks: {
            beginAtZero: true,
            maxTicksLimit: 100,
            // Create scientific notation labels
            callback: function (value, index, values) {
              return '$ ' + value.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
            }
          }
        }],
        xAxes: [{
          categoryPercentage: 1.0,
          barPercentage: 0.6,
        }]
      },
      tooltips: {
        callbacks: {
          label: this.tooltipFormatCureency
        }
      }
    };

    // NG2 INICIO opciones de barra STORE
    this.StoreBarChartOptions = {
      scaleShowVerticalLines: false,
      maintainAspectRatio: false,
      responsive: true,
      legend: {
        position: 'bottom',
      },
      scales: {
        yAxes: [{
          ticks: {
            beginAtZero: true,
            maxTicksLimit: 100,
            // Create scientific notation labels
            callback: function (value, index, values) {
              return '$ ' + value.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
            }
          }
        }],
        xAxes: [{
          categoryPercentage: 1.0,
          barPercentage: 0.6,
        }]
      },
      tooltips: {
        callbacks: {
          label: this.tooltipFormatCureency
        }
      }
    };

    this.lineChartColors = [{
      backgroundColor: 'rgba(77,83,96,0.2)',
      borderColor: 'rgba(77,83,96,1)',
      pointBackgroundColor: 'rgba(77,83,96,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(77,83,96,1)',
      pointRadius: 5
    }];

    this.lineChartOptions = {
      maintainAspectRatio: false,
      responsive: true,
      legend: {
        display: false,
      },
      scales: {
        yAxes: [{
          ticks: {
            beginAtZero: true,
            maxTicksLimit: 100,
            // Create scientific notation labels
            callback: function (value, index, values) {
              return '$ ' + value.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
            }
          }
        }]
      },
      tooltips: {
        callbacks: {
          label: this.tooltipFormatCureency
        }
      }
    };

    this.doughnutColors = [{ backgroundColor: ['#69C3FF', '#70ad47', '#ff8ba4'] }];

    this.doughnutOptions = {
      maintainAspectRatio: false,
      responsive: true,
      legend: {
        position: 'bottom',
      },
      pieceLabel: {
        showZero: false,
        render: 'label',
        fontSize: 15,
        position: 'outside',
      }
    };
  }

  onFilter() {
    if (this.starDate != null && this.endDate != null) {
      if (this.starDate.getDate() <= this.endDate.getDate()) {
          this.resetChartData();
          this.getData();
      } else {
        const dialogRef = this.dialog.open(DialogsAlertsComponent, {
          width: '400px',
          data: {
            message: 'La fecha de termino no puede ser menor a la fecha de inicio',
            title: 'Ocurrio un problema'
          }
        });
      }

    } else {
      const dialogRef = this.dialog.open(DialogsAlertsComponent, {
        width: '400px',
        data: {
          message: 'La fecha de inicio y fecha de termino son obligatorios',
          title: 'Ocurrio un problema'
        }
      });
    }
  }

  // MAXIMIZAR LA GRÁFICA
  onMaximizeChart(chartId: string) {


    this.resetDataChart(chartId);

    const chart = window.document.getElementById(chartId);
    const svgPanel = chart.getElementsByTagName('canvas')[0];

    this.chartHeight = svgPanel.getAttribute('height');
    this.chartWidth = svgPanel.getAttribute('width');

    svgPanel.removeAttribute('style');

    svgPanel.setAttribute('width', `${window.screen.width - 100}`);
    svgPanel.setAttribute('height', `${window.screen.height - 200}`);

  }

  // MINIMIZAR LA GRÁFICA
  onMinimizeChart(chartId: string) {

    const chart = window.document.getElementById(chartId);
    const svgPanel = chart.getElementsByTagName('canvas')[0];

    const parent = chart.parentElement;

    parent.classList.remove('animated');
    parent.classList.remove('fadeIn');

    svgPanel.setAttribute('width', `${this.chartWidth}`);
    svgPanel.setAttribute('height', `${this.chartHeight}`);
    svgPanel.setAttribute('style', 'width: 100%; height: 95%;');

    parent.classList.add('animated');
    parent.classList.add('fadeIn');


    this.resetDataChart(chartId);
  }

  // REGRESAR AL TAMAÑO INICIAL DE LAS GRÁFICAS
  resetDataChart(chartId: string): void {
    const arrayData: any = [];

    switch (chartId) {
      case 'chart-store':
        for (const row of this.dataStore) {
          arrayData.push(row);
        }
        this.storeBarChartData = arrayData;
        this.expandChartStores = !this.expandChartStores;
        break;
      case 'chart-platform':
        for (const row of this.dataPlatform) {
          arrayData.push(row);
        }
        this.PlatformBarChartData = arrayData;
        this.expandChartPlatform = !this.expandChartPlatform;
        break;
      case 'chart-quantity':
        this.lineChartData = [];

        for (const row of this.dataQuantity) {
          arrayData.push(row);
        }
        this.lineChartData = arrayData;
        this.expandChartQuantity = !this.expandChartQuantity;
        break;
      case 'chart-status':
        for (const row of this.dataStatusGraph) {
          arrayData.push(row);
        }
        this.doughnutChartData = arrayData;
        this.expendChartStatus = !this.expendChartStatus;
        break;
    }
  }

  // OBTENER EL LISTADO DE LAS TRANSACCIONES
  getData(): void {

    if (!this.working) {
      this.working = true;
    }

    this.transactionService
      .filterTransaction(this.starDate, this.endDate, this.storeId, this.platformId)
      .subscribe(
        res => {

          if (res.data) {

            this.transansactions = JSON.parse(JSON.stringify(res.data));

            for (let r of this.transansactions) {
              switch (r.statusPiorpi) {
                case 1:
                  r['statusDescriptionPiorpi'] = 'CORRECTO';
                  break;
                case 2:
                  r['statusDescriptionPiorpi'] = 'PENDIENTE';
                  break;
                case 3:
                  r['statusDescriptionPiorpi'] = 'ALERTA';
                  break;
              }
            }

            let data = res.data.filter(x => x.autorizationId != null);

            if ( data.length > 0) {

              this.totalMoney = data.map(t => parseFloat(t.amount)).reduce((s, c) => s + c);
            this.totalTransaction = data.length ? data.length : 0;

            let self = this;

              self.initPlatformChart(data);
              self.initStoremChart(data);
              self.initTimeLimeChart(data);
              self.initStatuschart(data);

            } else {
              this.resetNoData();
            }

          } else {
            this.resetNoData();
          }
          this.working = false;
        },
        err => {
          this.working = false;
        }
      );

  }

  // OBTENER EL LISTADO DE LOS ESTATUS
  getStatus(): void {
    this.transactionService
      .filterStatus(this.starDate, this.endDate, this.storeId, this.platformId)
      .subscribe(
        res => {
          this.initStatuschart(res);
          this.working = false;
        });
  }

  // DESCARGAR EL DETALLE DE LAS TRANSACCIONEs
  exportTransactionsData(): void {

    const options = {
      fieldSeparator: ',',
      decimalseparator: '.',
      showLabels: true,
      showTitle: true,
      title: `Transacciones de ${this.utilities.dateFormatDDMMYYYY(this.starDate)} - ${this.utilities.dateFormatDDMMYYYY(this.endDate)}`,
      useBom: true,
      headers: ['#', 'Fecha Aplicación', 'Total', 'Nombre Cuentahabiente',
        'Correo electrónico', 'Autorizacion', 'IdTienda', 'Tienda', 'IdPlataforma',
        'Plataforma', 'IdUsuario', 'OrdeCompra', 'Operación de caja', 'Fecha de Actualización',
        'Id Estatus PIORPI', 'Detalle PIORPI', 'Estatus PIORPI']
    };
    // tslint:disable-next-line:no-unused-expression
    new Angular5Csv(this.transansactions, 'transacciones', options);

  }

  /***************************************************************************************/
  /* GRÁFICA DE PLATAFORMAS                                                              */
  /************************************************************************************* */

  // INICIALIZAR LOS DATOS DE LA GRÁFICA DE PLATAFORMA
  initPlatformChart(res: any) {



    let grouped = this.utilities.groupBy(res, t => t.decriptionPlatform);
    grouped.forEach((element, key) => {
      let total = element.map(t => parseFloat(t.amount))
        .reduce((s, c) => s + c);
      this.dataPlatformDetails.push({ y: key, a: total });
      this.dataPlatform.push({ 'data': [total], 'label': key });
    });

    this.PlatformBarChartData = this.dataPlatform;

  }

  onOpenDialogPlatformDetail() {
    const DIALOG = this.dialog.open(DialogsTableComponent, {
      width: '420px',
      height: '400px',
      data: {
        title: 'Monto por plataforma',
        col01Title: 'Plataforma',
        col02Title: 'Total',
        largeTable: false,
        label: 'y',
        value: 'a',
        currencyFormat: true,
        detail: this.dataPlatformDetails
      }
    });
  }

  // Genera el csv para los datos por plataforma
  exportPlatformData(data: any): void {
    this.dataStore = [];
    const options = {
      fieldSeparator: ',',
      decimalseparator: '.',
      showLabels: true,
      showTitle: true,
      title: 'MONTOS POR PLATAFORMA',
      useBom: true,
      headers: ['Plataforma', 'Total']
    };
    // estructura los datos para el formato de excel
    for (let index = 0; index < data.length; index++) {
      const element = data[index];
      this.dataStore.push({ 'Nombre': element['label'], 'Total': this.format2(element['data'][0], '$') });
    }
    // tslint:disable-next-line:no-unused-expression
    new Angular5Csv(this.dataStore, 'MONTOS POR PLATAFORMA', options);
  }

  /***************************************************************************************/
  /* GRÁFICA DE TIENDAS                                                                  */
  /************************************************************************************* */

  // INICIALIZAR LOS DATOS DE LA GRÁFICA DE TIENDA
  initStoremChart(res: any) {
    this.dataStore = [];
    this.dataStoreDetails = [];
    const grouped = this.utilities.groupBy(res, t => t.storeDescripcion);
    grouped.forEach((element, key) => {
      const total = element.map(t => parseFloat(t.amount))
        .reduce((s, c) => s + c);
      this.dataStoreDetails.push({ y: key, a: total });
      this.dataStore.push({ 'data': [total], 'label': key });
    });
    this.storeBarChartData = this.dataStore;
  }

  // MUESTRA LOS DATOS PARA GENERAR LA GRÁFICA EN UNA MODAL
  onOpenDialogStoreDetail() {
    const DIALOG = this.dialog.open(DialogsTableComponent, {
      width: '420px',
      height: '400px',
      data: {
        title: 'Monto por tienda',
        col01Title: 'Tienda',
        col02Title: 'Total',
        largeTable: false,
        label: 'y',
        value: 'a',
        currencyFormat: true,
        detail: this.dataStoreDetails
      }
    });
  }

  // Genera el csv para los datos por tienda
  exportStoreData(data: any): void {
    this.dataStore = [];
    const options = {
      fieldSeparator: ',',
      decimalseparator: '.',
      showLabels: true,
      showTitle: true,
      title: 'MONTO POR TIENDA',
      useBom: true,
      headers: ['Tienda', 'Total']
    };
    for (let index = 0; index < data.length; index++) {
      const element = data[index];

      this.dataStore.push({ 'Nombre': element['label'], 'Total': this.format2(element['data'][0], '$') });
    }
    // tslint:disable-next-line:no-unused-expression
    new Angular5Csv(this.dataStore, 'CANTIDADES POR TIENDA', options);
  }

  /***************************************************************************************/
  /* GRÁFICA DE MONTO POR DIA                                                            */
  /************************************************************************************* */

  // INICIALIZAR LOS DATOS DE LA GRÁFICA DE LINEA DE TIEMPO
  initTimeLimeChart(res: any): void {

    for (let r of res) {
      r.createDate = r.createDate.substring(0, 10);
    }

    const grouped = this.utilities.groupBy(res, t => t.createDate);
    this.dataQuantity = [];
    this.dataQuantityDetails = [];
    let labels: Array<any> = [];
    this.lineChartLabels = [];

    grouped.forEach((element, key) => {
      const total = element.map(t => parseFloat(t.amount))
        .reduce((s, c) => s + c);

      const label = key.substring(0, 10);

      this.dataQuantityDetails.push({ y: label, a: total });
      this.dataQuantity.push(total);

      labels.push([label]);
      this.lineExcelQuantity.push({ y: label, a: this.format2(total, '$') });
    });
    this.lineExcelData = this.lineExcelQuantity;
    this.lineChartLabels = labels;
    this.lineChartData = [{ 'data': this.dataQuantity, 'label': 'Total' }];
  }

  // MUESTRA LOS DATOS PARA GENERAR LA GRÁFICA EN UNA MODAL
  onOpenDialogDayDetail(): void {

    let heightDialog = '400px';
    let largeTable = false;

    if (this.dataQuantityDetails.length > 9) {
      heightDialog = '650px';
      largeTable = true;
    }

    const DIALOG = this.dialog.open(DialogsTableComponent, {
      width: '400px',
      height: heightDialog,
      data: {
        title: 'Monto por día',
        col01Title: 'Día',
        col02Title: 'Total',
        label: 'y',
        value: 'a',
        largeTable: largeTable,
        currencyFormat: true,
        detail: this.dataQuantityDetails
      }
    });
  }

  /***************************************************************************************/
  /* GRÁFICA DE ESTATUS                                                                  */
  /************************************************************************************* */

  // Genera el csv para los datos por plataforma
  exportDateDetailData(data: any): void {
    const options = {
      fieldSeparator: ',',
      decimalseparator: '.',
      showLabels: true,
      showTitle: true,
      title: 'MONTOS POR FECHA',
      useBom: true,
      headers: ['Fecha', 'Total']
    };
    // tslint:disable-next-line:no-unused-expression
    new Angular5Csv(data, 'MONTOS POR FECHA', options);
  }

  // INICIALIZAR DATOS DE LA GRÁFICA DE ESTATUS
  initStatuschart(res: any): void {

    this.dataStatusGraph = [];
    if (res.length) {

      this.dataStatusGraph = [
        res.filter(x => x.statusPiorpi === 1).length,
        res.filter(x => x.statusPiorpi === 2).length,
        res.filter(x => x.statusPiorpi === 3).length
      ];

      this.dataStatusDetail = [
        { label: 'Correcto', value: this.dataStatusGraph[0] },
        { label: 'Pendiente', value: this.dataStatusGraph[1] },
        { label: 'Alerta', value: this.dataStatusGraph[2] }
      ];
    }

    this.doughnutChartData = this.dataStatusGraph;
  }

  // MUESTRA LOS DATOS PARA GENERAR LA GRÁFICA EN UNA MODAL
  onOpenDialogStatusDetail() {
    const DIALOG = this.dialog.open(DialogsTableComponent, {
      width: '440px',
      height: '440px',
      data: {
        title: 'Estatus PIORPI de transacciones ',
        col01Title: 'Estatus',
        col02Title: 'No. Transacciones',
        largeTable: false,
        label: 'label',
        value: 'value',
        currencyFormat: false,
        detail: this.dataStatusDetail
      }
    });
  }

  // Genera el csv para los datos por plataforma
  exportStatusData(): void {
    this.exportTransactionsData();
  }

  // DESCARGA LA IMAGEN DE LA GRAFICA
  exportChartToImage(chartId: string, nameImage: string): void {

    html2canvas(window.document.getElementById(chartId)).then(function (canvas) {
      const img = canvas.toDataURL('image/png');

      let linkDownload = window.document.createElement('a');
      linkDownload.setAttribute('target', '_blank');
      linkDownload.setAttribute('download', nameImage);
      linkDownload.setAttribute('href', img);

      linkDownload.click();
      linkDownload.remove();

    });
  }

  /***************************************************************************************/
  /* Funciones de graficas y otras                                                       */
  /***************************************************************************************/
  // Formato de cantidades
  format2(n, currency) {
    return currency + ' ' + n.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
  }

  tooltipFormatCureency(tooltipItem, data) {
    let label = data.datasets[tooltipItem.datasetIndex].label || '';

    if (label) {
      label += ': ';
    }
    label += '$ ' + tooltipItem.yLabel.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    return label;
  }

  resetChartData() {

    this.transansactions = [];

    // DATOS DE GRAFICA DE LINEA
    this.lineChartData = [];

    // GRAFICA PLATAFORMA
    this.dataPlatform = [];
    this.dataPlatformDetails = [];
    this.PlatformBarChartData = [];

    // GRAFICA DE TIENDA
    this.storeBarChartData = [];
    this.dataStore = [];
    this.dataStoreDetails = [];

    // GRAFICA DE STATUS
    this.dataStatusGraph = [];
    this.doughnutChartData = [];
    this.dataStatus = [];

    this.totalMoney = 0.00;
    this.totalTransaction = 0;
  }

  syncStatus() {

    this.working = true;
    this.transactionService.syncStatusByDate(this.starDate, this.endDate).subscribe(
      res => {

        if (res.isSuccess) {
          for (let row of this.transansactions) {
            const transaction = res.data.filter(x => x.autorizationId === row.autorizationId);
            if (transaction.length) {
              row.cashOperationId = transaction[0].cashOperationId;
              row.validationDate = transaction[0].validationDate;
              row.statusPiorpi = transaction[0].statusPiorpi;
              row.StatusPiorpi = transaction[0].StatusPiorpi;
            }
          }
        }
        this.working = false;
      }
      , err => {
        this.working = false;
      });
  }

  onChangeStore(event: any) {

    let comparePlatform = this.compare(event.value, this.storeLast);

    if ((event.value.length === 1 && event.value[0] === 0) ||  (comparePlatform.length === 1 && comparePlatform[0] === 0) ) {

      this.storeId   = [];
      this.storeLast =[];
      for (let s of this.stores ) {
          this.storeId.push( s.value );
          this.storeLast.push( s.value);
      }

    } else if ( comparePlatform.indexOf(0) === -1 && comparePlatform.length > 0
                 &&  (this.stores.length - 1) === ( comparePlatform.length )) {

      this.storeId = [];
      this.storeLast = [];

    } else if ( comparePlatform.indexOf(0) > -1 && (this.stores.length - 1) === ( comparePlatform.length )) {

      this.storeId = [];
      this.storeLast =[];
      for (let s of comparePlatform ) {
        if ( s != 0 ) {
          this.storeId.push( s );
          this.storeLast.push(s);
        }
      }

    } else {

      this.storeLast = [];
      for(let v of event.value) {
        this.storeLast.push(v);
      }
    }

    this.setToolTipStore();
  }

  onChangePlatform(event: any) {

    let comparePlatform = this.compare(event.value, this.platfomLast);

    if ((event.value.length === 1 && event.value[0] === 0) ||  (comparePlatform.length === 1 && comparePlatform[0] === 0) ) {

      this.platformId  = [];
      this.platfomLast = [];
      for (let s of this.plataforms ) {
          this.platformId.push( s.value );
          this.platfomLast.push( s.value);
      }

    } else if ( comparePlatform.indexOf(0) === -1 &&  comparePlatform.length > 0
              && (this.plataforms.length - 1) === ( comparePlatform.length )) {

      this.platformId = [];
      this.platfomLast = [];

    } else if ( comparePlatform.indexOf(0) > -1 && (this.plataforms.length - 1) === ( comparePlatform.length )) {

      this.platformId  = [];
      this.platfomLast = [];
      for (let s of comparePlatform ) {
        if ( s != 0 ) {
          this.platformId.push( s );
          this.platfomLast.push(s);
        }
      }

    } else {
      this.platfomLast = [];
      for(let v of event.value) {
        this.platfomLast.push(v);
      }
    }

    this.setToolTipPlatform();

  }

  compare(arr1, arr2){

    const objMap = [];

  arr1.forEach((e1) => arr2.forEach((e2) => {
      if ( e1 !== e2 ) {
        if ( objMap.indexOf(e1) === -1) {
          objMap.push(e1);
        }
      }
    }
  ));

  return objMap;
}

  resetNoData() {

    this.totalMoney = 0.00;
              this.totalTransaction = 0;
              this.transansactions = [];

              const dialogRef = this.dialog.open(DialogsAlertsComponent, {
                width: '400px',
                data: {
                  message: 'No se encontrarón datos con los filtros seleccionados',
                  title: 'Filtrado de datos'
                }
              });
  }

  setToolTipStore (): void {
    this.toolTipStore = '';
    this.toolTipStore = this.createMessageToolTip(this.storeId, this.stores);
  }

  setToolTipPlatform (): void {
    this.toolTipPlatform = '';
    this.toolTipPlatform = this.createMessageToolTip(this.platformId, this.plataforms);
  }

  createMessageToolTip(values, elements): string {
    let message = '';
    values.forEach( e => {
      const el = elements.filter( x => x.value === e);
      if ( el && el[0].viewValue !== undefined ) {
        message += el[0].viewValue + ' , ';
      }
    });
    return message;
  }

}
