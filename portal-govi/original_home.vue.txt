<template>
  <v-container>
    <v-toolbar dense>
      <v-toolbar-title>Acceso directo</v-toolbar-title>
    </v-toolbar>
    <v-row>
      <v-col
        v-for="(item, i) in items"
        :key="i"
        cols="12"
        sm="6"
        md="4"
        lg="3"
      >
        <v-card
          color="#26c6da"
          dark
          max-width="400"
          :to="item.path"
        >
          <v-card-title>
            <v-icon
              large
              left
            >
              link
            </v-icon>
            <span class="title font-weight-light">{{item.desc}}</span>
          </v-card-title>

          <v-card-text class="headline font-weight-bold">

          </v-card-text>

          <v-card-actions>
            <v-list-item class="grow">

            </v-list-item>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>

export default {
  name: 'Home',
  data () {
    return {
      items: [],
      oItems: []
    }
  },
  created () {
    this.$router.options.routes[0].children.filter(x => x.name != 'Home' && x.name != 'About').forEach(route => {
      this.items.push({
        name: route.name,
        path: route.path,
        desc: route.desc
      })
    })
  },
}
</script>
