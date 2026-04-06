import Vue from 'vue';
import Vuetify from 'vuetify/lib';
import es from 'vuetify/es5/locale/es';

Vue.use(Vuetify);

export default new Vuetify({
  theme: {
    dark: true,
    themes: {
      dark: {
        primary: '#1B84FF',
        secondary: '#0cb9c5',
        accent: '#00D8FF',
        success: '#28C76F',
        info: '#00CFE8',
        warning: '#f8a102',
        error: '#EA5455',
        background: '#192838',
      },
      light: {
        primary: '#7367F0',
        secondary: '#E91E63',
        accent: '#00D8FF',
        success: '#28C76F',
        info: '#00CFE8',
        warning: '#FF9F43',
        error: '#EA5455',
        background: '#F8F8F8',
      },
    },
    options: {
      customProperties: true,
    },
  },
  lang: {
    locales: { es },
    current: 'es',
  },
  icons: {
    iconfont: 'md',
  },
});
