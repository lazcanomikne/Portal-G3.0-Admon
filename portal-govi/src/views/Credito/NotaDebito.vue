<template>
  <v-container>
    <v-toolbar dense class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">Nota debito</v-toolbar-title>
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
          Documentos Generados en SAP | Total: {{ response.length }}
        </v-card-title>
        <v-card-text>
          <v-data-table
            dense
            :items="response"
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
      <p>Generando documentos</p>
      <v-progress-linear
        v-model="value"
        :active="overlay"
        :query="true"
        height="25"
      >
        <template v-slot:default="{ value }">
          <strong
            >Procesando {{ (value * rows.length) / 100 }} /
            {{ rows.length }}</strong
          >
        </template>
      </v-progress-linear>
    </v-overlay>
  </v-container>
</template>

<script>
import { mapActions } from "vuex";
import xlsx from "xlsx";
import { mixin } from "../../mixin";

export default {
  name: "AjusteEntrada",
  data: () => ({
    selectedSucursal: null,
    selectedFile: undefined,
    overlay: false,
    response: [],
    value: 0,
    query: false,
    showAlert: false,
    responseColumns: [
      { text: "DocEntry", value: "docEntry" },
      { text: "CardCode", value: "cliente" },
      { text: "Cantidad", value: "cantidad", align: "right" },
      { text: "Precio", value: "precio", align: "right" },
    ],
  }),
  mixins: [mixin],
  methods: {
    ...mapActions("notas", ["postNotaDebito"]),
    async EnviarSap() {
      this.overlay = true;
      this.query = true;
      this.value = 0;
      for (const element of this.rows) {
        this.value += 100 / this.rows.length;
        const info = {
          Notas: [element],
        };
        const res = await this.postNotaDebito(info);
        if (res) {
          this.response.push(res.data[0]);
        }
      }

      this.overlay = false;
      this.rows = [];
      this.selectedFile = undefined;
      this.showAlert = true;
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
};
</script>

<style></style>../../mixin
