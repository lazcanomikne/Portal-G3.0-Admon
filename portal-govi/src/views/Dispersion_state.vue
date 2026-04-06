<template>
  <v-container>
    <v-toolbar dense id="p" class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">Actualizar dispersión</v-toolbar-title>
      <v-spacer> </v-spacer>
      <v-btn
        depressed
        class="glass-btn"
        @click="generarHandlerEvent"
        :disabled="!selectedToFile.length"
      >
        Actualizar
      </v-btn>
    </v-toolbar>
    <div>
      <v-row dense>
        <v-col cols="12" sm="3" md="3">
          <v-dialog
            ref="dialogStart"
            v-model="modalStart"
            :return-value.sync="dateStart"
            persistent
            width="290px"
          >
            <template v-slot:activator="{ on, attrs }">
              <v-text-field
                v-model="dateStart"
                label="Fecha inicial"
                prepend-icon="event"
                class="glass-input"
                readonly
                v-bind="attrs"
                v-on="on"
              ></v-text-field>
            </template>
            <v-date-picker v-model="dateStart" scrollable>
              <v-spacer></v-spacer>
              <v-btn text color="primary" @click="modalStart = false">
                Cancel
              </v-btn>
              <v-btn text color="primary" @click="saveDateStart">
                OK
              </v-btn>
            </v-date-picker>
          </v-dialog>
        </v-col>
        <v-col cols="12" sm="3" md="3">
          <v-dialog
            ref="dialogEnd"
            v-model="modalEnd"
            :return-value.sync="dateEnd"
            persistent
            width="290px"
          >
            <template v-slot:activator="{ on, attrs }">
              <v-text-field
                v-model="dateEnd"
                label="Fecha final"
                prepend-icon="event"
                class="glass-input"
                readonly
                v-bind="attrs"
                v-on="on"
              ></v-text-field>
            </template>
            <v-date-picker v-model="dateEnd" scrollable>
              <v-spacer></v-spacer>
              <v-btn text color="primary" @click="modalEnd = false">
                Cancel
              </v-btn>
              <v-btn text color="primary" @click="saveDateEnd">
                OK
              </v-btn>
            </v-date-picker>
          </v-dialog>
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <v-select
            label="Seleccione la Sociedad o Empresa"
            dense
            outlined
            class="glass-input"
            :items="sociedades"
            v-model="selectedSociedad"
            :item-text="getSociedadText"
            item-value="u_CompnyName"
            return-object
            @input="cargarDatos3"
            :disabled="selectedToFile.length > 0"
            full-width
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
            :items="transferencias"
            :search="search"
            hide-default-footer
            disable-pagination
            disable-sort
            fixed-header
            item-key="name"
            class="glass-table elevation-1"
            ref="table"
            :height="tableHeight"
            id="tablemain"
          >
            <template v-slot:top>
              <v-banner sticky icon="search" flat>
                <v-text-field
                  v-model="search"
                  label="Buscar transferencia"
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
            <template v-slot:[`item.u_dispersion`]="{ item }">
              <v-switch disabled v-model="item.u_dispersion"></v-switch>
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
            :height="tableHeight"
            id="tabledetalle"
          >
            <template v-slot:top>
              <v-row no-gutters>
                <v-col cols="12" sm="6" md="8">
                  <v-btn
                    rounded
                    icon
                    title="Eliminar todos"
                    @click="selectedToFile = []"
                  >
                    <v-icon>delete</v-icon>
                  </v-btn>
                  {{ selectedToFile.length }} seleccionadas | Total:
                  {{ getTotal | currency }}
                </v-col>
              </v-row>
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
            <template v-slot:[`item.u_dispersion`]="{ item }">
              <v-switch v-model="item.u_dispersion"></v-switch>
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </div>
    <!-- Dialog -->
    <v-dialog v-model="showdialog" persistent width="700">
      <v-card>
        <v-card-title class="headline grey lighten-2">
          Resultado
        </v-card-title>
        <v-card-text>
          <v-list subheader two-line>
            <v-subheader inset>Archivos</v-subheader>

            <v-list-item v-for="(file, index) in archivos" :key="index">
              <v-list-item-avatar>
                <v-icon class="grey lighten-1" dark>
                  attachment
                </v-icon>
              </v-list-item-avatar>

              <v-list-item-content>
                <v-list-item-title v-text="file"></v-list-item-title>
              </v-list-item-content>

              <v-list-item-action>
                <v-btn
                  icon
                  :href="'ftp://192.168.1.206/' + file"
                  target="_blank"
                >
                  <v-icon color="grey lighten-1">information</v-icon>
                </v-btn>
              </v-list-item-action>
            </v-list-item>
          </v-list>
        </v-card-text>
        <v-card-actions>
          <v-btn text color="primary" @click="showdialog = false">Cerrar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-overlay style="text-align: center" :value="overlay">
      <p>Generando dispersion de pagos</p>
      <v-progress-circular indeterminate size="64"></v-progress-circular>
    </v-overlay>
    <v-overlay style="text-align: center" :z-index="zIndex" :value="showResult">
      <p>{{ Respuesta }}</p>
      <v-btn depressed color="primary" @click="showResult = false">
        Aceptar
      </v-btn>
    </v-overlay>
  </v-container>
</template>

<script>
import { mapActions } from "vuex";
//const setClass = new Set()
export default {
  name: "Dispersion",
  data: () => ({
    archivos: [],
    Respuesta: "",
    dialog: false,
    dialogDelete: false,
    editedIndex: -1,
    dispersion: "no",
    selectedSociedad: null,
    search: "",
    selected: [],
    selectedToFile: [],
    loadSucural: false,
    loadRest: false,
    loadTable: false,
    overlay: false,
    isGenerate: true,
    modalStart: false,
    modalEnd: false,
    dateStart: new Date().toISOString().substr(0, 10),
    dateEnd: new Date().toISOString().substr(0, 10),
    zIndex: 0,
    headers: [
      { text: "Documento", value: "docNum" },
      { text: "Dispersion", value: "u_dispersion" },
      { text: "Total", value: "docTotal", align: "right" },
      { text: "Add", value: "actions" },
    ],
    headers2: [
      { text: "Borrar", value: "actions" },
      { text: "Dispersion", value: "u_dispersion" },
      { text: "DocNum", value: "docNum" },
      { text: "DocTotal", value: "docTotal" },
      { text: "CardName", value: "cardName" },
      { text: "Sociedad", value: "sociedad" },
      { text: "VATRegNum", value: "vatRegNum" },
      { text: "BankCtlKey", value: "bankCtlKey" },
      { text: "MandateID", value: "mandateID" },
      { text: "Account", value: "account" },
      { text: "DflAccount", value: "dflAccount" },
      { text: "JrnlMemo", value: "jrnlMemo" },
      { text: "County", value: "county" },
      { text: "LicTradNum", value: "licTradNum" },
      { text: "IVA", value: "IVA" },
      { text: "E_Mail", value: "e_Mail" },
      { text: "DocDate", value: "docDate" },
      { text: "DocEntry", value: "docEntry" },
    ],
    showdialog: false,
    showResult: false,
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
      "getAllTransfersDispersion",
      "updateDispersion",
      "limpiar",
      "getSociedades",
    ]),
    getSociedadText(item) {
      return `${item.code} - ${item.u_CompnyName}`;
    },
    saveDateStart() {
      this.$refs.dialogStart.save(this.dateStart);
    },
    saveDateEnd() {
      this.$refs.dialogEnd.save(this.dateEnd);
    },
    cargarDatos3() {
      this.loadTable = true;
      this.getAllTransfersDispersion({
        sociedad: this.selectedSociedad.u_DB,
        fecha1: this.dateStart.replaceAll("-", ""),
        fecha2: this.dateEnd.replaceAll("-", ""),
      }).then((res) => {
        this.loadTable = false;
      });
    },
    handlerEvent(e) {
      if (this.$refs.table._data.internalCurrentItems.length > 0) {
        let item = this.$refs.table._data.internalCurrentItems[0];
        let newItem = Object.assign({}, item);
        item.u_dispersion = !item.u_dispersion;
        this.selectedToFile.push(item);
        this.selectedToFile = [...new Set(this.selectedToFile)];
        this.search = "";
      } else alert("Tranferencia no encontrada, intente de nuevo.");
    },
    addItem(item) {
      let newItem = Object.assign({}, item);
      item.u_dispersion = !item.u_dispersion;
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
    generarHandlerEvent() {
      this.overlay = true;
      let data = {
        transferencias: this.selectedToFile.map((t) => {
          return {
            docNum: t.docEntry,
            u_dispersion: t.u_dispersion ? "si" : "no",
          };
        }),
        sociedad: this.selectedSociedad.u_DB,
      };
      this.updateDispersion(data)
        .then((res) => {
          this.overlay = false;
          alert("Actualización realizada");
          this.cancelProcess();
        })
        .catch((err) => {
          this.overlay = false;
          console.log(err);
        });
    },
    cancelProcess() {
      this.search = "";
      this.selectedToFile = [];
      this.limpiar();
    },
  },
  computed: {
    tableHeight() {
      return window.innerHeight - 0;
    },
    sociedades() {
      return this.$store.state.dispersion.sociedades;
    },
    transferencias() {
      const t = this.$store.state.dispersion.transferenciasDispersion;
      return t;
      //return t.filter((t) => t.u_dispersion === this.dispersion);
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
