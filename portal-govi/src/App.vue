<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script>

export default {
  name: 'App',
  mounted () {
    this.$vuetify.theme.isDark = (localStorage.getItem('dark') == 'true')
  },
};
</script>

<style>
/* --- GLOBAL RESET & TYPOGRAPHY (Applies to ALL modes) --- */
.v-application {
  font-family: 'Poppins', sans-serif !important;
}

/* --- TYPOGRAPHY SCALE --- */
/* Headings */
.v-application .display-1, .v-application h1 { font-size: 1.75rem !important; font-weight: 600 !important; letter-spacing: -0.5px !important; }
.v-application .display-2, .v-application h2 { font-size: 1.375rem !important; font-weight: 600 !important; letter-spacing: -0.5px !important; }
.v-application .display-3, .v-application h3 { font-size: 1.125rem !important; font-weight: 600 !important; letter-spacing: 0px !important; }
.v-application .headline, .v-application h4   { font-size: 1rem !important; font-weight: 500 !important; letter-spacing: 0.25px !important; }
.v-application .title, .v-application h5      { font-size: 0.875rem !important; font-weight: 500 !important; letter-spacing: 0.15px !important; }
.v-application .subtitle-1, .v-application h6 { font-size: 0.75rem !important; font-weight: 500 !important; letter-spacing: 0.15px !important; }

/* Body Text */
.v-application .body-1 { font-size: 0.875rem !important; font-weight: 400 !important; letter-spacing: 0.5px !important; line-height: 1.5 !important; }
.v-application .body-2 { font-size: 0.75rem !important; font-weight: 400 !important; letter-spacing: 0.25px !important; }
.v-application .caption { font-size: 0.625rem !important; font-weight: 400 !important; letter-spacing: 0.4px !important; opacity: 0.7; }

/* --- MODERN SCROLLBARS --- */
::-webkit-scrollbar {
  width: 12px;
  height: 12px;
}
::-webkit-scrollbar-track {
  background: transparent;
}
::-webkit-scrollbar-thumb {
  border-radius: 10px;
  background: rgba(120, 120, 120, 0.3);
  border: 2px solid transparent;
  background-clip: content-box;
}
.theme--dark ::-webkit-scrollbar-thumb {
  background-color: rgba(248, 161, 2, 0.3);
}
.theme--light ::-webkit-scrollbar-thumb {
  background-color: rgba(0, 0, 0, 0.2);
}
::-webkit-scrollbar-thumb:hover {
  background-color: rgba(248, 161, 2, 0.6) !important;
}

/* --- THEME: DARK MODE --- */
.v-application.theme--dark {
  background-color: #192838 !important;
  color: #EAEFF4 !important;
}
.theme--dark.v-application .display-1, 
.theme--dark.v-application h1, .theme--dark.v-application h2, 
.theme--dark.v-application h3, .theme--dark.v-application h4, .theme--dark.v-application h5 {
  color: #EAEFF4 !important;
}
/* Dark Mode Borders & Surface */
.theme--dark.v-sheet.v-card, 
.theme--dark.v-sheet.v-dialog {
  background-color: #152332 !important;
  border: 1px solid #333F55 !important;
}
.theme--dark.v-navigation-drawer {
  background-color: #152332 !important;
  border-right: 1px solid #333F55 !important;
}

/* --- THEME: LIGHT MODE --- */
.v-application.theme--light {
  background-color: #F8F8F8 !important; /* Soft White/Grey */
  color: rgba(46, 38, 61, 0.87) !important; /* Standard Dark Text */
}
.theme--light.v-application .display-1, 
.theme--light.v-application h1, .theme--light.v-application h2, 
.theme--light.v-application h3, .theme--light.v-application h4, .theme--light.v-application h5 {
  color: #2c3e50 !important; /* Blue-ish Dark Grey */
}
/* Light Mode distinct card shadow/border */
.theme--light.v-sheet.v-card {
  box-shadow: 0 10px 30px -5px rgba(0, 0, 0, 0.08), 0 4px 10px -5px rgba(248, 161, 2, 0.1) !important;
  border: 1px solid rgba(0, 0, 0, 0.05) !important;
  border-bottom: 3px solid rgba(0, 0, 0, 0.1) !important; /* 3D thickness */
  background-color: #ffffff !important;
}
.theme--light.v-navigation-drawer {
  background-color: #ffffff !important;
  border-right: 1px solid rgba(0, 0, 0, 0.05) !important;
  box-shadow: 5px 0 25px rgba(0, 0, 0, 0.03) !important;
}

/* 3D Button Effect for Light Mode */
.theme--light .v-btn:not(.v-btn--flat):not(.v-btn--text):not(.v-btn--outlined):not(.v-btn--round) {
  border-bottom: 3px solid rgba(0, 0, 0, 0.15) !important;
  transition: all 0.1s ease !important;
}
.theme--light .v-btn:active {
  transform: translateY(2px) !important;
  border-bottom-width: 1px !important;
}

/* --- SHAPE & COMPONENT STRUCTURE (Universal) --- */
.v-sheet.v-card, 
.v-sheet.v-dialog,
.v-menu__content {
  border-radius: 12px !important;
}
.v-menu__content { border: none !important; }

/* Buttons */
.v-btn:not(.v-btn--round) {
  border-radius: 8px !important;
  font-weight: 600 !important;
  text-transform: none !important;
}

/* Inputs */
.v-text-field--outlined fieldset {
  border-radius: 8px !important;
}
/* Dark Input Colors */
.theme--dark.v-text-field--outlined fieldset {
  border-color: rgba(255, 255, 255, 0.2) !important;
}
.theme--dark.v-input input, .theme--dark.v-input textarea { color: #fff !important; }
.theme--dark.v-label { color: rgba(255,255,255,0.7) !important; }

/* Light Input Colors */
.theme--light.v-text-field--outlined fieldset {
  border-color: rgba(0, 0, 0, 0.15) !important;
}

/* Focus State */
.v-input--is-focused .v-input__slot fieldset {
  border-color: #7367F0 !important;
}

/* --- UTILITIES --- */
.btn-gradient {
  background: linear-gradient(90deg, #7367F0 0%, #CE9FFC 100%) !important;
  color: white !important;
  border: none !important;
  box-shadow: 0 4px 15px 0 rgba(115, 103, 240, 0.4) !important;
}

/* Base Glass Header (Shared Structure) */
.v-app-bar.glass-header {
  box-shadow: none !important;
  transition: all 0.3s ease !important;
}

/* --- DARK MODE APP BAR (Crystal Orange) --- */
.theme--dark .v-app-bar.glass-header {
  background: rgba(248, 161, 2, 0.25) !important;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.1) 0%, rgba(248, 161, 2, 0.4) 100%) !important;
  backdrop-filter: blur(25px) saturate(200%) !important;
  -webkit-backdrop-filter: blur(25px) saturate(200%) !important;
  border-bottom: 1px solid rgba(255, 255, 255, 0.3) !important;
  color: #FFFFFF !important;
}

/* Ensure inner content is transparent to avoid edge artifacts */
.v-app-bar.glass-header .v-toolbar__content {
  background: transparent !important;
}
.theme--dark .v-app-bar.glass-header .v-icon,
.theme--dark .v-app-bar.glass-header .v-btn,
.theme--dark .v-app-bar.glass-header .v-toolbar__title {
  color: #FFFFFF !important;
  text-shadow: 0 2px 4px rgba(0,0,0,0.2) !important;
}

/* --- LIGHT MODE APP BAR (3D Floating White) --- */
.theme--light .v-app-bar.glass-header {
  background-color: #FFFFFF !important;
  /* 3D Elevation Shadow */
  box-shadow: 0 4px 20px -5px rgba(248, 161, 2, 0.3), 0 8px 15px -5px rgba(0, 0, 0, 0.05) !important;
  border-bottom: 3px solid #f8a102 !important; /* 3D Orange accent base */
}
.theme--light .v-app-bar.glass-header .v-toolbar__content {
  background-color: #FFFFFF !important;
}

/* High Contrast Text for Light Mode */
.theme--light .v-app-bar.glass-header .v-toolbar__title,
.theme--light .v-app-bar.glass-header .v-icon,
.theme--light .v-app-bar.glass-header .v-btn {
  color: #152332 !important; /* Dark Blue for perfect readability */
  font-weight: 700 !important;
}

/* Navigation Styles (Pill Shape) */
.logo-brand {
  filter: drop-shadow(0 2px 4px rgba(0,0,0,0.1));
  transition: transform 0.3s ease !important;
}
.logo-brand:hover {
  transform: scale(1.05) !important;
}
.brand-title {
  letter-spacing: 1.5px !important;
  text-transform: uppercase;
  font-size: 0.875rem !important;
}
.v-navigation-drawer .v-list-item {
  border-radius: 24px !important; /* Pill shape */
  margin: 4px 8px !important;
}

/* Sidebar Active State - Scoped by Theme */
.theme--dark .v-navigation-drawer .v-list-item--active {
  background: linear-gradient(90deg, #f8a102 0%, #e69500 100%) !important;
  color: #152332 !important;
}

.theme--light .v-navigation-drawer .v-list-item--active {
  background-color: #f8a102 !important; /* Solid Orange (No gradient) */
  color: #FFFFFF !important;
  box-shadow: 0 4px 10px rgba(248, 161, 2, 0.4) !important;
  border: 1px solid rgba(0,0,0,0.05) !important;
}

.v-navigation-drawer .v-list-item--active .v-icon {
  color: inherit !important;
}

/* --- SIDEBAR MINI VARIANT FIX --- */
.v-navigation-drawer--mini-variant .v-list-item {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  margin: 8px auto !important;
  padding: 0 !important;
  width: 44px !important; 
  height: 44px !important; /* Forces a perfect square */
  min-height: 44px !important;
  border-radius: 12px !important;
}

/* Force the icon container to have NO internal Vuetify margins */
.v-navigation-drawer--mini-variant .v-list-item__icon {
  margin: 0 !important;
  padding: 0 !important;
  align-self: center !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  min-width: 0 !important; /* Fix for Vuetify mini min-width */
  width: 100% !important;
  height: 100% !important;
}

/* Ensure the icon itself is not offset */
.v-navigation-drawer--mini-variant .v-list-item__icon .v-icon {
  margin: 0 !important;
  padding: 0 !important;
  line-height: normal !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

/* --- DATA TABLES REDESIGN --- */
.v-data-table {
  border-radius: 12px !important;
  background-color: transparent !important;
}

/* Header Styling */
.v-data-table thead th {
  font-weight: 600 !important;
  text-transform: uppercase !important;
  font-size: 0.75rem !important;
  letter-spacing: 1px !important;
  height: 56px !important;
}

/* Cell Styling */
.v-data-table tbody td {
  font-size: 0.75rem !important;
  height: 48px !important; /* Slightly more compact rows too */
  border-bottom-width: 1px !important;
}

/* Dark Mode Table */
.theme--dark.v-data-table thead th {
  background-color: #1E2D3E !important; /* Slightly lighter than surface */
  color: #EAEFF4 !important;
  border-bottom: 1px solid #333F55 !important;
}
.theme--dark.v-data-table tbody tr:hover:not(.v-data-table__expanded__content):not(.v-data-table__empty-wrapper) {
  background-color: #2a3447 !important; /* Hover state */
}
.theme--dark.v-data-table tbody td {
  border-bottom: 1px solid #333F55 !important;
  color: #EAEFF4 !important;
}
/* Striped effect (optional, subtle) */
.theme--dark.v-data-table tbody tr:nth-of-type(odd) {
  background-color: rgba(255, 255, 255, 0.02);
}

/* Light Mode Table */
.theme--light.v-data-table thead th {
  background-color: #F8F9FA !important;
  color: #5e6e82 !important;
  border-bottom: 1px solid #EAEFF4 !important;
}
.theme--light.v-data-table tbody tr:hover:not(.v-data-table__expanded__content):not(.v-data-table__empty-wrapper) {
  background-color: #F2F4F8 !important;
}
.theme--light.v-data-table tbody td {
  border-bottom: 1px solid #EAEFF4 !important;
  color: #2c3e50 !important;
}

/* --- ICONS: MODERN LINEAR STYLE --- */
.v-icon.material-icons {
  font-family: 'Material Icons Outlined' !important;
  /* Optional: Make them slightly smaller/finer if needed */
  font-weight: normal; 
}

/* --- MODULE TOOLBARS (HEADERS) --- */
/* Target toolbars inside the main content area */
.v-main .v-toolbar {
  border-radius: 16px !important;
  margin-bottom: 24px !important; /* Space below header */
  height: 64px !important; /* Perfect balance? */
}

/* Ensure inner content matches the new height */
.v-main .v-toolbar .v-toolbar__content {
  height: 64px !important;
}

/* Dark Mode Module Header - CRYSTAL STYLE */
.v-application.theme--dark .v-main .v-toolbar {
  background: rgba(21, 35, 50, 0.4) !important; /* Translucent Surface */
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.05) 0%, rgba(21, 35, 50, 0.6) 100%) !important;
  backdrop-filter: blur(20px) saturate(120%) !important;
  -webkit-backdrop-filter: blur(20px) saturate(120%) !important;
  color: #EAEFF4 !important;
  box-shadow: 0 8px 32px -4px rgba(248, 161, 2, 0.45) !important; /* Vibrant Orange Shadow */
  border: 1px solid rgba(255, 255, 255, 0.1) !important;
}

/* Light Mode Module Header - 3D STYLE */
.theme--light.v-main .v-toolbar {
  background-color: #FFFFFF !important;
  box-shadow: 0 6px 20px -5px rgba(0,0,0,0.1), 0 4px 10px -5px rgba(248, 161, 2, 0.3) !important;
  border-bottom: 3px solid #f8a102 !important; /* 3D Accent */
  color: #152332 !important;
}

/* Title Styling inside Module Header */
.v-main .v-toolbar__title {
  font-weight: 700 !important;
  font-size: 1.125rem !important;
  letter-spacing: 0.5px !important;
}

/* --- FOOTER STYLING (#272727 Replacement) --- */
.theme--dark.v-footer {
  background-color: #f8a102 !important; /* Requested Orange */
  color: #192838 !important; /* Dark text for contrast */
  font-weight: 600 !important;
}
</style>