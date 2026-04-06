<template>
  <v-container class="py-6">
    <!-- Header with modern styling -->
    <div class="d-flex align-center mb-6">
      <v-icon color="primary" class="mr-3">dashboard_customize</v-icon>
      <h1 class="text-h5 font-weight-bold mb-0 secondary--text text--darken-2">
        Accesos Directos
      </h1>
      <v-spacer></v-spacer>
    </div>

    <!-- Menu Sections -->
    <div v-for="(menu, index) in doneMenu" :key="index" class="mb-8">
      <div class="d-flex align-center mb-3">
        <v-divider class="mr-4"></v-divider>
        <span class="text-overline font-weight-black grey--text">{{ menu.Tag }}</span>
        <v-divider class="ml-4"></v-divider>
      </div>

      <v-row>
        <v-col 
          v-for="(item, i) in menu.SubMenu" 
          :key="i" 
          cols="12" sm="6" md="4" lg="3" 
          class="pa-2"
        >
          <v-card 
            class="shortcut-card elevation-2 d-flex flex-column" 
            @click="navegar(item)"
          >
            <v-card-text class="d-flex align-center grow py-4 px-4">
              <v-avatar color="primary lighten-5" size="48" class="mr-4">
                <v-icon color="primary" size="28">
                  {{ getIcon(item.Tag) }}
                </v-icon>
              </v-avatar>
              
              <div class="flex-column">
                <div class="text-subtitle-2 font-weight-bold brand-secondary--text mb-0 line-height-tight">
                  {{ item.Tag }}
                </div>
                <div class="text-caption grey--text text--darken-1 mt-1">
                  Acceso rápido
                </div>
              </div>

              <v-spacer></v-spacer>
              <v-icon small class="chevron-icon grey--text text--lighten-1">chevron_right</v-icon>
            </v-card-text>
            
            <!-- Bottom decorative line -->
            <div class="accent-line"></div>
          </v-card>
        </v-col>
      </v-row>
    </div>
  </v-container>
</template>

<script>
import { mapGetters } from "vuex";

export default {
  name: "Home",
  computed: {
    ...mapGetters("config", ["doneMenu"]),
  },
  methods: {
    navegar (item) {
      this.$store.commit("config/SET_CANCREATE", item.CanCreate);
      this.$store.commit("informes/SET_TITLE", item.Tag);
      this.$router.push({ path: item.Path, params: item.CanCreate });
    },
    getIcon(tag) {
      const lowerTag = tag.toLowerCase();
      // Tesorería / Bancos
      if (lowerTag.includes('banco') || lowerTag.includes('cheque') || lowerTag.includes('caja')) return 'account_balance';
      if (lowerTag.includes('pago') || lowerTag.includes('transfer')) return 'payments';
      if (lowerTag.includes('egreso') || lowerTag.includes('gasto')) return 'outbox';
      if (lowerTag.includes('ingreso') || lowerTag.includes('deposito')) return 'move_to_inbox';
      
      // Crédito / Ventas
      if (lowerTag.includes('credito') || lowerTag.includes('cartera')) return 'credit_card';
      if (lowerTag.includes('venta') || lowerTag.includes('factura')) return 'point_of_sale';
      if (lowerTag.includes('cotiza')) return 'request_quote';
      if (lowerTag.includes('inmueble')) return 'domain';
      
      // Reportes / Análisis
      if (lowerTag.includes('analisis') || lowerTag.includes('cuadro')) return 'analytics';
      if (lowerTag.includes('reporte') || lowerTag.includes('informe')) return 'description';
      if (lowerTag.includes('grafica') || lowerTag.includes('dashboard')) return 'leaderboard';
      
      // Configuración / Otros
      if (lowerTag.includes('config') || lowerTag.includes('ajuste')) return 'settings_suggest';
      if (lowerTag.includes('usuario')) return 'manage_accounts';
      if (lowerTag.includes('descarga') || lowerTag.includes('layout')) return 'file_download';
      
      return 'arrow_forward'; // Default
    }
  }
};
</script>

<style scoped>
.shortcut-card {
  border-radius: 12px !important;
  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1) !important;
  cursor: pointer;
  height: 100%;
  position: relative;
  overflow: hidden;
  border: 1px solid rgba(0, 0, 0, 0.05) !important;
}

.shortcut-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 12px 20px rgba(0,0,0,0.1) !important;
}

.shortcut-card:hover .chevron-icon {
  transform: translateX(3px);
  color: var(--v-primary-base) !important;
}

.shortcut-card:hover .v-avatar {
  background-color: var(--v-primary-base) !important;
}

.shortcut-card:hover .v-avatar .v-icon {
  color: white !important;
}

.accent-line {
  height: 3px;
  width: 0;
  background: var(--v-primary-base);
  transition: width 0.3s ease;
}

.shortcut-card:hover .accent-line {
  width: 100%;
}

.line-height-tight {
  line-height: 1.2;
}

.text-overline {
  font-size: 0.7rem !important;
  letter-spacing: 2px !important;
}

/* Theme specific overrides */
.theme--dark .shortcut-card {
  background: rgba(255, 255, 255, 0.05) !important;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1) !important;
}

.theme--dark .shortcut-card:hover {
  background: rgba(255, 255, 255, 0.08) !important;
  box-shadow: 0 0 20px rgba(248, 161, 2, 0.2) !important;
}

.theme--dark .v-avatar {
  background: rgba(248, 161, 2, 0.1) !important;
}
</style>
