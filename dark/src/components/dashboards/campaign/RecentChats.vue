<script setup lang="ts">
import { ref } from 'vue';
import { recentChatsData } from '@/_mockApis/components/dashboards/MinimalData';
import { SendIcon } from 'vue-tabler-icons';

const inputMessage = ref('');
const chats = ref([...recentChatsData]);

const sendMessage = () => {
  if (inputMessage.value.trim() !== '') {
    chats.value.push({
      color: 'lightwarning',
      name: '',
      desc: inputMessage.value.trim(),
      time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
      img: '', // Optional: Add an avatar for the user
      chat: false,
    });
    inputMessage.value = ''; // Clear the input field
  }
};
</script>


<template>
    <v-card elevation="10">
        <v-card-text>
            <h3 class="card-title">Recent Chats</h3>
            <div class="mt-6">
                <perfect-scrollbar class="scrollnavbar" style="max-height:310px">
                    <div class="d-flex ga-4" v-for="list in chats" :key="list.name + list.time">
                        <v-avatar size="45" v-if="list.chat">
                            <img :src="list.img" alt="user" width="45" />
                        </v-avatar>
                        <div class="d-sm-flex mb-8 ga-6 align-center w-100" v-if="list.chat">
                            <div>
                                <h5 class="text-subtitle-1 textSecondary font-weight-medium mb-2">{{ list.name }}</h5>
                                <div :class="'bg-' + list.color" class="py-4 px-5 rounded-pill rounded-ts-0 text-15 opacity-80">
                                    {{ list.desc }}
                                </div>
                            </div>
                            <p class="text-subtitle-2 textSecondary ms-auto mt-6 text-no-wrap">{{ list.time }}</p>
                        </div>

                        <div class="d-sm-flex justify-end mb-8 align-center w-100" v-if="!list.chat">
                            <div class="d-flex ga-5 align-center">
                                <div :class="'bg-' + list.color" class="py-4 px-5 rounded-pill rounded-te-0 text-15 opacity-80">
                                    {{ list.desc }}
                                </div>
                                <p class="text-subtitle-2 textSecondary ms-auto text-no-wrap">{{ list.time }}</p>
                            </div>
                        </div>
                    </div>
                </perfect-scrollbar>
            </div>
        </v-card-text>
        <div class="py-7 px-6 border-t">
            <div class="d-flex align-center justify-space-between">
                <v-text-field
                    v-model="inputMessage"
                    variant="solo"
                    hide-details
                    color="primary"
                    class="shadow-none ms-n2"
                    density="compact"
                    placeholder="Type a Message"
                    @keydown.enter="sendMessage"
                ></v-text-field>
                <v-btn icon color="primary" size="50" @click="sendMessage">
                    <SendIcon size="20" />
                </v-btn>
            </div>
        </div>
    </v-card>
</template>


