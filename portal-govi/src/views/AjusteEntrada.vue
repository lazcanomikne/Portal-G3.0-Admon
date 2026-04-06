<template>
  <v-container>
    <v-toolbar dense class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">Ajustes de entrada de inventario</v-toolbar-title>
      <v-spacer> </v-spacer>
      <v-btn
        depressed
        class="glass-btn"
        :disabled="!selectedFile"
        style="margin-right: 10px"
        @click="EnviarSap"
      >
        Enviar a SAP
      </v-btn>
    </v-toolbar>
    <div>
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
        <v-col class="d-flex" cols="6">
          <v-file-input
            label="Buscar comprobante"
            outlined
            dense
            class="glass-input"
            @change="onFileChange2"
            v-model="selectedFile2"
          ></v-file-input>
        </v-col>
      </v-row>
      <v-row>
        <v-col class="d-flex" cols="6">
          <v-select
            label="Cedis"
            dense
            outlined
            class="glass-input"
            :items="cedis"
            v-model="selectedSucursal"
            :item-text="getSucursalText"
            return-object
            item-value="bplId"
          ></v-select>
        </v-col>
        <v-col class="d-flex" cols="6">
          <v-text-field dense outlined class="glass-input" label="Motivo ajuste" v-model="motivo">
          </v-text-field>
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
          </v-data-table>
        </v-col>
      </v-row>
    </div>
    <!--  -->
    <v-dialog v-model="showAlert" persistent width="600">
      <v-card>
        <v-card-title class="headline">
          Documento Generado en SAP: {{ response.DocNum }} -
          {{ response.DocTotal | currency }}
        </v-card-title>
        <v-card-text>
          <v-data-table
            dense
            :items="response.DocumentLines"
            :headers="responseColumns"
            hide-default-footer
            disable-pagination
            fixed-header
            disable-sort
            class="elevation-1"
            style="max-height: 300px"
            height="300px"
          >
          </v-data-table>
        </v-card-text>
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
      <p>Generando ajuste</p>
      <v-progress-circular indeterminate size="64"></v-progress-circular>
    </v-overlay>
  </v-container>
</template>

<script>
import xlsx from "xlsx";
import { mapActions } from "vuex";
import { mixin } from "../mixin";

export default {
  name: "AjusteEntrada",
  data: () => ({
    selectedSucursal: null,
    selectedFile: undefined,
    selectedFile2: undefined,
    motivo: "",
    overlay: false,
    response: [],
    showAlert: false,
    responseColumns: [
      { text: "Producto", value: "ItemCode" },
      { text: "Descripción", value: "ItemDescription" },
      { text: "Ajuste", value: "Quantity", align: "right" },
    ],
  }),
  mixins: [mixin],
  methods: {
    ...mapActions("ajustes", ["postAjuste", "getCedis"]),
    EnviarSap() {
      this.overlay = true;
      const info = {
        Ajustes: this.rows,
        Sucursal: this.selectedSucursal,
        Motivo: this.motivo,
        Tipo: "entrada",
      };
      this.postAjuste(info)
        .then((res) => {
          if (res) {
            this.overlay = false;
            this.rows = [];
            this.selectedFile = undefined;
            this.response = res.data;
            this.showAlert = true;
          }
        })
        .catch((err) => {
          this.overlay = false;
          this.response = err.data;
          alert(this.response);
          console.error(err);
        })
        .finally(() => {
          this.overlay = false;
        });
    },
    getSucursalText(item) {
      return `${item.bplName} - ${item.bplFrName}`;
    },
    onFileChange2(event) {},
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
          ws.forEach((element) => {
            element["PRODUCTO"] =
              typeof element["PRODUCTO"] === "string"
                ? element["PRODUCTO"]
                : "" + element["PRODUCTO"] + "";
            if (!element.hasOwnProperty("DESCRIPCION")) {
              element["DESCRIPCION"] = "";
            }
          });
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
    cedis() {
      return this.$store.state.ajustes.cedis;
    },
  },
  mounted() {
    this.getCedis();
  },
};
</script>

<style></style>
