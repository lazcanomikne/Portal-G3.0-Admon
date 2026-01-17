<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { todoListData } from '@/_mockApis/components/dashboards/ClassicData';
const checkedTasks = ref<{ [key: string]: boolean }>({});
onMounted(() => {
    todoListData.forEach((task) => {
        checkedTasks.value[task.title] = false;
    });
});
const toggleTask = (title: string) => {
    checkedTasks.value[title] = !checkedTasks.value[title];
};

const showModal = ref(false);
</script>

<template>
    <VCard elevation="10">
        <v-card-text class="pb-2">
            <div class="d-flex justify-space-between align-center">
                <div>
                    <h2 class="card-title mb-1">To Do list</h2>
                    <h5 class="card-subtitle">List of your next task to complete</h5>
                </div>
                <v-btn color="primary" @click="showModal = true" class="mt-sm-0 mt-3" flat rounded="pill">Add Task</v-btn>
            </div>
            <div class="mt-8">
                <div class="todo-list" v-for="list in todoListData" :key="list.title">
                    <div class="d-sm-flex ga-6">
                        <div class="ms-n2 mt-n2">
                            <v-checkbox color="primary" hide-details v-model="checkedTasks[list.title]"></v-checkbox>
                        </div>
                        <div>
                            <div class="d-flex ga-3 align-center">
                                <h3 class="text-h6" :class="{ strikethrough: checkedTasks[list.title] }">
                                    {{ list.title }}
                                </h3>
                                <v-chip :color="list.badgebg" size="small">{{ list.status }}</v-chip>
                            </div>
                            <p class="text-14 textSecondary one-line mt-1">{{ list.desc }}</p>
                        </div>
                        <div class="ms-auto">
                            <v-img width="56" height="56" aspect-ratio="16/9" class="rounded-sm mt-sm-0 mt-2" cover :src="list.img"></v-img>
                        </div>
                    </div>
                </div>
            </div>
        </v-card-text>
    </VCard>

    <v-dialog v-model="showModal" max-width="500px">
        <v-card>
            <v-card-title class="pa-4 bg-primary">Add Task</v-card-title>
            <v-card-text class="pb-0">
                <v-form>
                    <v-label>Task name</v-label>
                    <v-text-field placeholder="Enter Task name"></v-text-field>
                    <v-label>Assign To</v-label>
                    <v-select  :items="['Sachin', 'Steve', 'Jack', 'Wiliam', 'Dona']" variant="outlined"></v-select>
                </v-form>
            </v-card-text>
            <v-card-actions class="ga-3 pa-4">
                <v-btn color="error" rounded="pill" variant="flat" @click="showModal = false">Close</v-btn>
                <v-btn color="primary" rounded="pill" variant="flat">Submit</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<style scoped>
.strikethrough {
    text-decoration: line-through;
}
</style>
