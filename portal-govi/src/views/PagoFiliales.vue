<template>
  <v-container>
    <v-toolbar dense class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">Pago filiales</v-toolbar-title>
      <v-spacer> </v-spacer>
      <v-btn
        depressed
        class="glass-btn"
        :disabled="!rows.length"
        style="margin-right: 10px"
        @click="EnviarSap"
      >
        Enviar a SAP
      </v-btn>
    </v-toolbar>
    <div>
      <v-row>
        <v-col class="d-flex" cols="6" md="6">
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
            @input="cargarDatos"
          ></v-select>
        </v-col>
        <v-col class="d-flex" cols="3">
          <v-select
            label="Surcural"
            dense
            outlined
            class="glass-input"
            :items="sucursales"
            v-model="selectedSucursal"
            :item-text="getSucursalText"
            return-object
            item-value="bplName"
          ></v-select>
        </v-col>
        <v-col class="d-flex" cols="3">
          <v-text-field dense outlined class="glass-input" label="Proveedor" v-model="proveedor">
          </v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col class="d-flex" cols="6">
          <v-file-input
            label="Buscar archivo"
            outlined
            dense
            class="glass-input"
            @change="onFileChange"
            v-model="selectedFile"
          ></v-file-input>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12" md="12">
          <v-data-table
            dense
            :items="rows"
            :headers="columns"
            hide-default-footer
            disable-pagination
            fixed-header
            disable-sort
            class="glass-table elevation-1"
            style="max-height: 500px"
            height="500px"
          >
            <template v-slot:top>
              Total de la tranferencia: {{ getTotal | currency }}
            </template>
          </v-data-table>
        </v-col>
      </v-row>
    </div>
    <!--  -->
    <v-dialog v-model="showAlert" persistent width="600">
      <v-card>
        <v-card-title class="headline">
          Documento SAP {{ response.DocNum }} | Total:
          {{ response.TransferSum | currency }}
        </v-card-title>
        <v-card-actions>
          <v-btn
            text
            color="primary"
            @click="
              showAlert = false;
              response = [];
            "
            >Cerrar</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-overlay style="text-align: center" :value="overlay">
      <p>Generando documentos</p>
      <v-progress-circular indeterminate size="64"></v-progress-circular>
    </v-overlay>
  </v-container>
</template>

<script>
import xlsx from "xlsx";
import { mapActions } from "vuex";
import { mixin } from "../mixin";

export default {
  name: "PagoFiliales",
  data: () => ({
    selectedSucursal: null,
    selectedFile: undefined,
    overlay: false,
    response: [],
    value: 0,
    query: false,
    showAlert: false,
    loadSucural: false,
    selectedSociedad: null,
    selectedSucursal: null,
    proveedor: "",
  }),
  mixins: [mixin],
  methods: {
    ...mapActions("dispersion", [
      "getSociedades",
      "getSucursales",
      "postPagosFiliales",
    ]),
    getSociedadText(item) {
      return `${item.code} - ${item.u_CompnyName}`;
    },
    getSucursalText(item) {
      return `${item.bplName} - ${item.bplFrName}`;
    },
    cargarDatos(sociedad) {
      this.loadSucural = true;
      this.getSucursales(sociedad.u_DB).then((res) => {
        this.loadSucural = false;
      });
    },
    async EnviarSap() {
      try {
        this.overlay = true;
        const info = {
          info: this.rows,
          sociedad: this.selectedSociedad.u_DB,
          sucursal: this.selectedSucursal.bplName,
          proveedor: this.proveedor,
        };
        const res = await this.postPagosFiliales(info);
        if (res) {
          this.response = res.data;
        }
        this.overlay = false;
        this.rows = [];
        this.selectedFile = undefined;
        this.showAlert = true;
      } catch (error) {
        this.overlay = false;
        alert(error.data.error.message.value);
      }
    },
    onFileChange(event) {
      if (!this.selectedFile) {
        this.rows = [];
        return;
      }
      if (!/\.(xls|xlsx)$/.test(this.selectedFile.name.toLowerCase())) {
        return alert(
          "The upload format is incorrect. Please upload xls or xlsx format"
        );
      }
      const fileReader = new FileReader();
      fileReader.onload = (ev) => {
        try {
          const data = ev.target.result;
          const XLSX = xlsx;
          const workbook = XLSX.read(data, {
            type: "binary",
          });
          const wsname = workbook.SheetNames[0]; // Take the first sheet，wb.SheetNames[0] :Take the name of the first sheet in the sheets
          const ws = XLSX.utils.sheet_to_json(workbook.Sheets[wsname]); // Generate JSON table content，wb.Sheets[Sheet名]    Get the data of the first sheet

          const a = workbook.Sheets[workbook.SheetNames[0]];
          const headers = this.getHeader(a);
          this.setTable(headers, ws);
        } catch (e) {
          return alert("Read failure!");
        }
      };
      fileReader.readAsBinaryString(this.selectedFile);
    },
  },
  computed: {
    sociedades() {
      return this.$store.state.dispersion.sociedades;
    },
    sucursales() {
      return this.$store.state.dispersion.sucursales;
    },
    getTotal() {
      return this.rows.reduce((a, b) => a + (b["TOTAL"] || 0), 0);
    },
  },
  mounted() {
    this.getSociedades();
  },
};
</script>

<style></style>
