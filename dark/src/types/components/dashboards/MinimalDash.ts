import type { TablerIconComponent } from 'vue-tabler-icons';
type browserStates = {
    img: string;
    name: string;
    visit: string;
    color: string;
};

type recentChats = {
    color: string;
    img: string;
    name: string;
    desc: string;
    time: string;
    chat:boolean
};

export type { browserStates, recentChats };
