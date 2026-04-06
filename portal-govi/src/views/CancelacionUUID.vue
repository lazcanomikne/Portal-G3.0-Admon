<template>
  <v-container>
    <v-toolbar dense class="glass-toolbar mb-4">
      <v-toolbar-title class="font-weight-bold">Cancelación Masiva UUID</v-toolbar-title>
      <v-spacer> </v-spacer>
      <v-btn
        depressed
        class="glass-btn"
        :disabled="!selectedFile"
        style="margin-right: 10px"
        @click="EnviarSap"
      >
        Enviar
      </v-btn>
    </v-toolbar>
    <div>
      <v-row>
        <v-col class="d-flex" cols="10">
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
    <v-dialog v-model="showAlert" persistent>
      <v-card>
        <v-card-title class="headline">
          Proceso terminado con exito
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
      <p>Procesando archivo...</p>
      <v-progress-circular indeterminate size="64"></v-progress-circular>
    </v-overlay>
  </v-container>
</template>

<script>
import { mapActions } from "vuex";
import xlsx from "xlsx";
import { mixin } from "../mixin";

export default {
  name: "CancelacionUUID",
  data: () => ({
    selectedFile: undefined,
    overlay: false,
    response: [],
    showAlert: false,
  }),
  mixins: [mixin],
  methods: {
    ...mapActions("cancelacion", ["postCancelacion"]),
    EnviarSap() {
      this.overlay = true;
      this.postCancelacion(this.rows)
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

<style></style>
