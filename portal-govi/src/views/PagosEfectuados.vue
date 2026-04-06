<template>
  <v-container>
    <v-toolbar dense class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">Pagos Efectuados</v-toolbar-title>
      <v-spacer> </v-spacer>
      <v-btn
        depressed
        class="glass-btn"
        @click="generaLote"
        :disabled="!selectedToFile.length"
        style="margin-right: 10px"
      >
        Generar
      </v-btn>
    </v-toolbar>
    <div>
      <!-- Sociedad & Sucursal -->
      <v-row>
        <v-col class="d-flex" cols="6">
          <v-select
            label="Seleccione Empresa"
            dense
            outlined
            class="glass-input"
            :items="sociedades"
            v-model="selectedSociedad"
            :item-text="getSociedadText"
            item-value="u_CompnyName"
            return-object
            @input="cargarDatos"
            :disabled="selectedToFile.length > 0"
          ></v-select>
        </v-col>
        <v-col class="d-flex" cols="6" md="6">
          <v-select
            v-show="selectedSociedad.u_DB == 'SBOGOVI'"
            label="Surcural"
            dense
            outlined
            class="glass-input"
            :items="sucursales"
            v-model="selectedSucursal"
            :item-text="getSucursalText"
            return-object
            item-value="bplName"
            @input="cargarDatos2"
            :disabled="selectedToFile.length > 0"
          ></v-select>
        </v-col>
      </v-row>
      <!-- Operacion & Cuenta -->
      <v-row>
        <v-col class="d-flex" cols="6">
          <v-select
            v-show="selectedSociedad.u_DB == 'SBOGOVI'"
            label="Cuenta origen"
            dense
            outlined
            class="glass-input"
            :items="cuentas"
            :disabled="selectedToFile.length > 0"
            :item-text="getCuentaText"
            item-value="glAccount"
            @input="cargarDatos3"
          ></v-select>
        </v-col>
      </v-row>
      <!-- Tablas -->
      <v-row>
        <v-col cols="4" md="4" class="">
          <v-data-table
            dense
            v-if="!loadTable"
            v-model="selected"
            :headers="headers"
            :items="pasivos"
            :search="search"
            :custom-filter="customFilter"
            hide-default-footer
            disable-pagination
            disable-sort
            fixed-header
            item-key="name"
            class="glass-table elevation-1"
            ref="table"
            style="max-height: 500px"
            height="400px"
          >
            <template v-slot:top>
              <v-banner sticky icon="search" flat>
                <v-text-field
                  v-model="search"
                  label="Buscar movimiento"
                  class="glass-input mx-4"
                  @keydown.stop.enter="handlerEvent"
                ></v-text-field>
              </v-banner>
            </template>
            <template v-slot:[`item.actions`]="{ item }">
              <v-icon small @click="addItem(item)">
                forward
              </v-icon>
            </template>
            <template v-slot:[`item.docTotal`]="{ item }">
              <span> {{ item.docTotal | currency }} </span>
            </template>
          </v-data-table>
          <v-skeleton-loader
            v-if="loadTable"
            class="mx-auto"
            type="table"
          ></v-skeleton-loader>
        </v-col>
        <v-col class="" cols="8" md="8">
          <v-data-table
            dense
            :headers="headers2"
            :items="selectedToFile"
            class="glass-table elevation-1"
            hide-default-footer
            disable-pagination
            disable-sort
            :fixed-header="true"
            style="max-height: 500px"
            height="500px"
            id="tabledetalle"
          >
            <template v-slot:top>
              <v-btn rounded icon @click="selectedToFile = []">
                <v-icon>delete</v-icon>
              </v-btn>
              {{ selectedToFile.length }} seleccionadas | Total:
              {{ getTotal | currency }}
              <v-dialog v-model="dialogDelete" max-width="600px">
                <v-card>
                  <v-card-title class="headline"
                    >¿Esta seguro que desea borrar esta factura?</v-card-title
                  >
                  <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue darken-1" text @click="closeDelete"
                      >Cancelar</v-btn
                    >
                    <v-btn color="blue darken-1" text @click="deleteItemConfirm"
                      >OK</v-btn
                    >
                    <v-spacer></v-spacer>
                  </v-card-actions>
                </v-card>
              </v-dialog>
            </template>
            <template v-slot:[`item.actions`]="{ item }">
              <v-icon small @click="deleteItem(item)">
                delete
              </v-icon>
            </template>
            <template v-slot:[`item.docTotal`]="{ item }">
              <span> {{ item.docTotal | currency }} </span>
            </template>
            <template v-slot:[`item.cardName`]="{ item }">
              <span :title="item.cardName">
                {{ item.cardName | textcrop(18) }}
              </span>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </div>
    <!-- Dialog -->
    <v-dialog v-model="alertlote" persistent width="400">
      <v-card>
        <v-card-title class="headline">
          Resultado
        </v-card-title>
        <v-card-text>
          <v-btn @click="alertlote = false">{{ archivo }}</v-btn>
        </v-card-text>
        <v-card-actions>
          <v-btn text color="primary" @click="alertlote = false">Cerrar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-overlay :value="overlay">
      <v-progress-circular indeterminate size="64"></v-progress-circular>
    </v-overlay>
  </v-container>
</template>

<script>
import { mapActions } from "vuex";
export default {
  name: "Dispersion",
  data: () => ({
    archivo: "",
    archivos: [],
    dialog: false,
    dialogDelete: false,
    editedIndex: -1,
    selectedSociedad: {},
    selectedSucursal: null,
    operacion: null,
    search: "",
    selected: [],
    selectedToFile: [],
    loadSucural: false,
    loadRest: false,
    loadTable: false,
    overlay: false,
    headers: [
      { text: "Pasivo", value: "docNum" },
      { text: "Saldo", value: "docTotal", align: "right" },
      { text: "Add", value: "actions" },
    ],
    headers2: [
      { text: "Borrar", value: "actions" },
      //{ text: "Sociedad", value: "sociedad" },
      { text: "DocNum", value: "docNum" },
      { text: "SaldoPendiente", value: "saldoPendiente" },
      { text: "CardName", value: "cardName" },
      { text: "VATRegNum", value: "vatRegNum" },
      { text: "AcctCode", value: "acctCode" },
      { text: "AcctName", value: "acctName" },
      { text: "SeriesName", value: "seriesName" },
      //{ text: "Series", value: "series" },
      //{ text: "DocTotal", value: "docTotal" },
      //{ text: "PaidToDate", value: "paidToDate" },
      //{ text: "TransferSum", value: "transferSum" },
      //{ text: "CardCode", value: "cardCode" },
      //{ text: "DocEntry", value: "docEntry" },
      //{ text: "BPLId", value: "bPLId" },
      //{ text: "BPLName", value: "bPLName" },
      //{ text: "JournalRemarks", value: "journalRemarks" },
      //{ text: "Comments", value: "comments" },
    ],
    alertlote: false,
    alertuno: false,
  }),
  watch: {
    dialog(val) {
      val || this.close();
    },
    dialogDelete(val) {
      val || this.closeDelete();
    },
  },
  methods: {
    ...mapActions("dispersion", [
      "getSucursales",
      "getSociedades",
      "getCuentas",
      "getPasivos",
      "postPasivos",
      "limpiar",
    ]),
    getSociedadText(item) {
      return `${item.code} - ${item.u_CompnyName}`;
    },
    getSucursalText(item) {
      return `${item.bplName} - ${item.bplFrName}`;
    },
    getOperacionText(item) {
      return `${item.d} - ${item.n}`;
    },
    getCuentaText(item) {
      return `${item.glAccount} - ${item.acctName}`;
    },
    cargarDatos(sociedad) {
      this.loadSucural = true;
      if (sociedad.u_DB != "SBOGOVI") {
        this.cargarDatos3("");
      }
      this.getSucursales(sociedad.u_DB).then((res) => {
        this.loadSucural = false;
      });
    },
    cargarDatos2(sucursal) {
      this.loadRest = true;
      this.getCuentas({
        sociedad: this.selectedSociedad.u_DB,
        sucursal: sucursal.bplName,
      }).then((res) => {
        this.loadRest = false;
      });
    },
    cargarDatos3(cuenta) {
      this.loadTable = true;
      this.getPasivos({
        sociedad: this.selectedSociedad.u_DB,
        sucursal: this.selectedSucursal?.bplName || "",
        cuenta,
      }).then((res) => {
        this.loadTable = false;
      });
    },
    handlerEvent(e) {
      if (this.$refs.table._data.internalCurrentItems.length > 0) {
        this.selectedToFile.push(
          this.$refs.table._data.internalCurrentItems[0]
        );
        this.selectedToFile = [...new Set(this.selectedToFile)];
        this.search = "";
      } else alert("Tranferencia no encontrada, intente de nuevo.");
    },
    addItem(item) {
      this.selectedToFile.push(item);
      this.selectedToFile = [...new Set(this.selectedToFile)];
    },
    deleteItem(item) {
      this.editedIndex = this.selectedToFile.indexOf(item);
      this.dialogDelete = true;
    },
    deleteItemConfirm() {
      this.selectedToFile.splice(this.editedIndex, 1);
      this.closeDelete();
    },
    closeDelete() {
      this.dialogDelete = false;
      this.$nextTick(() => {
        this.editedIndex = -1;
      });
    },
    generaLote() {
      this.overlay = true;
      let data = {
        sociedad: this.selectedSociedad.u_DB,
        info: this.prepareData(),
      };
      this.postPasivos(data)
        .then((res) => {
          if (res != null) {
            this.archivo = res;
            this.overlay = false;
            this.alertlote = true;
            this.cancelProcess();
          }
        })
        .catch((err) => {
          this.overlay = false;
          alert(err);
        });
    },
    cancelProcess() {
      this.search = "";
      this.selectedToFile = [];
      this.limpiar();
    },
    prepareData() {
      const onlydata = this.selectedToFile.map((item) => {
        const {
          cardCode,
          bplId,
          bplName,
          series,
          acctCode,
          journalRemarks,
          comments,
          docEntry,
          saldoPendiente,
        } = item;
        return {
          cardCode,
          bplId,
          bplName,
          series,
          transferAccount: acctCode,
          journalRemarks,
          comments,
          docEntry,
          sumApplied: saldoPendiente,
        };
      });
      let temp = [];
      onlydata.forEach((e) => {
        const {
          cardCode,
          bplId,
          bplName,
          series,
          transferAccount,
          journalRemarks,
          comments,
          ...data
        } = e;
        let d = temp.find(
          (o) =>
            e.cardCode == o.cardCode &&
            e.bplId == o.bplId &&
            e.series == o.series &&
            e.transferAccount == o.transferAccount
        );
        if (d) {
          d.paymentInvoices.push({
            docEntry: data.docEntry,
            sumApplied: data.sumApplied,
          });
        } else {
          temp.push({
            cardCode,
            bplId,
            bplName,
            series,
            transferAccount,
            journalRemarks,
            remarks: comments,
            paymentInvoices: [data],
          });
        }
      });
      const result = temp.map((item) => {
        return {
          ...item,
          transferSum: item?.paymentInvoices.reduce(
            (acc, v) => acc + v.sumApplied,
            0
          ),
        };
      });
      return result;
    },
    customFilter(docNum, search, filter) {
      return filter.docNum.toString().indexOf(search) !== -1;
    },
  },
  computed: {
    sociedades() {
      return this.$store.state.dispersion.sociedades;
    },
    sucursales() {
      return this.$store.state.dispersion.sucursales;
    },
    cuentas() {
      return this.$store.state.dispersion.cuentas;
    },
    pasivos() {
      return this.$store.state.dispersion.pasivos;
    },
    getValuesFromSet() {
      return this.selectedToFile.entries().next().value;
    },
    getTotal() {
      return this.selectedToFile.reduce((a, b) => a + (b["docTotal"] || 0), 0);
    },
  },
  mounted() {
    this.limpiar();
    this.getSociedades();
  },
};
</script>

<style scoped></style>
