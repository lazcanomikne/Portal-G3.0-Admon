import xlsx from "xlsx";

export const mixin = {
  data: () => ({
    rows: [],
    columns: [],
  }),
  methods: {
    getHeader (sheet) {
      const XLSX = xlsx;
      const headers = [];
      const range = XLSX.utils.decode_range(sheet["!ref"]); // worksheet['!ref'] Is the valid range of the worksheet
      let C;
      /* Get cell value start in the first row */
      const R = range.s.r; //Line / / column C
      let i = 0;
      for (C = range.s.c; C <= range.e.c; ++C) {
        var cell =
          sheet[
          XLSX.utils.encode_cell({ c: C, r: R })
          ]; /* Get the cell value based on the address  find the cell in the first row */
        var hdr = "UNKNOWN" + C; // replace with your desired default
        // XLSX.utils.format_cell Generate cell text value
        if (cell && cell.t) hdr = XLSX.utils.format_cell(cell);
        if (hdr.indexOf("UNKNOWN") > -1) {
          if (!i) {
            hdr = "__EMPTY";
          } else {
            hdr = "__EMPTY_" + i;
          }
          i++;
        }
        headers.push(hdr);
      }
      return headers;
    },
    setTable (headers, excellist) {
      const tableTitleData = []; // Store table header data
      headers.forEach((_, i) => {
        tableTitleData.push({
          text: _.toUpperCase(),
          value: _.toUpperCase(),
        });
      });
      this.columns = tableTitleData;
      this.rows = excellist;
    },
  },
};
