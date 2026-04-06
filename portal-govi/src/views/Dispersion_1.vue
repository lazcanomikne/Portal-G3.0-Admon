<template>
  <v-container>
    <v-toolbar dense id="p" class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">Dispersion de pagos</v-toolbar-title>
      <v-spacer> </v-spacer>
      <v-btn
        depressed
        class="glass-btn"
        @click="generaUnoxUno"
        :disabled="!selectedToFile.length"
      >
        Generar
      </v-btn>
    </v-toolbar>
    <div>
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
            hide-default-footer
            disable-pagination
            disable-sort
            :fixed-header="true"
            :height="tableHeight"
            id="tabledetalle"
            class="glass-table elevation-1"
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
                <v-col cols="6" md="4" align-self="end">
                  <v-checkbox
                    dense
                    style="margin-top: 0px"
                    v-model="isGenerate"
                    :label="`Generar archivos: ${isGenerate ? 'Si' : 'No'}`"
                  ></v-checkbox>
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
    search: "",
    selected: [],
    selectedToFile: [],
    loadSucural: false,
    loadRest: false,
    loadTable: false,
    overlay: false,
    isGenerate: true,
    zIndex: 0,
    headers: [
      { text: "Documento", value: "docNum" },
      { text: "Total", value: "docTotal", align: "right" },
      { text: "Add", value: "actions" },
    ],
    headers2: [
      { text: "Borrar", value: "actions" },
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
      "getAllTransfers",
      "generarTxtUnoxUno",
      "limpiar",
    ]),
    cargarDatos3() {
      this.loadTable = true;
      this.getAllTransfers().then((res) => {
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
    generaUnoxUno() {
      this.overlay = true;
      let data = {
        transferencias: this.selectedToFile,
        g: this.isGenerate ? 1 : 0,
      };
      this.generarTxtUnoxUno(data)
        .then((res) => {
          if (res != null) {
            if (!Array.isArray(res)) {
              this.overlay = false;
              this.showResult = true;
              this.Respuesta = res;
            } else {
              this.archivos = res;
              this.overlay = false;
              this.showdialog = true;
            }
            this.cancelProcess();
          }
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
      this.getAllTransfers();
    },
  },
  computed: {
    tableHeight() {
      return window.innerHeight - 0;
    },
    transferencias() {
      return this.$store.state.dispersion.transferencias;
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
    this.getAllTransfers();
  },
};
</script>

<style scoped></style>
