import type { browserStates, recentChats } from "@/types/components/dashboards/MinimalDash";
import { MessageDotsIcon, ShoppingCartIcon, StarIcon } from 'vue-tabler-icons';
import img1 from '@/assets/images/browser/chrome-logo.svg';
import img2 from '@/assets/images/browser/firefox-logo.svg';
import img3 from '@/assets/images/browser/safari-logo.svg';
import img4 from '@/assets/images/browser/internet-logo.svg';
import img5 from '@/assets/images/browser/opera-logo.svg';
import img6 from '@/assets/images/browser/netscape-logo.svg';
import img7 from '@/assets/images/browser/micro-edge-logo.svg';

const browserStatesData: browserStates[] = [
  {
    img: img1,
    name: "Google Chrome",
    visit: "23%",
    color: "primary",
  },
  {
    img: img2,
    name: "Mozila Firefox",
    visit: "15%",
    color: "error",
  },
  {
    img: img3,
    name: "Apple Safari",
    visit: "07%",
    color: "warning",
  },
  {
    img: img4,
    name: "Internet Explorer",
    visit: "23%",
    color: "success",
  },
  {
    img: img5,
    name: "Opera mini",
    visit: "23%",
    color: "primary",
  },
  {
    img: img7,
    name: "Microsoft edge",
    visit: "23%",
    color: "info",
  },
  {
    img: img6,
    name: "	Netscape Navigator",
    visit: "44%",
    color: "error",
  },
];

import user1 from '@/assets/images/profile/user-2.jpg';
import user2 from '@/assets/images/profile/user-3.jpg';
import user3 from '@/assets/images/profile/user-4.jpg';
import user4 from '@/assets/images/profile/user-5.jpg';

const recentChatsData: recentChats[] = [
  {
    color: 'lightprimary',
    name: 'Angelo Rhodes',
    desc: 'Well we have good budget for the project',
    time:'11:00 am',
    img:user1,
    chat:true
  },
  {
    color: 'lightsuccess',
    name: 'Bianca Doe',
    desc: 'It’s Great opportunity to work.',
    time:'10:57 am',
    img:user2,
    chat:true
  },
  {
    color: 'lightwarning',
    name: '',
    desc: 'I would love to join the team.',
    time:'10:58 am',
    img:user3,
    chat:false
  },
  {
    color: 'lightwarning',
    name: '',
    desc: 'Whats budget of the new project.',
    time:'10:59 am',
    img:user3,
    chat:false
  },
  {
    color: 'lightprimary',
    name: 'Angelo Rhodes',
    desc: 'Well we have good budget for the project',
    time:'11:57 am',
    img:user4,
    chat:true
  },
  {
    color: 'lightprimary',
    name: 'Angelo Rhodes',
    desc: 'Lorem Ipsum is simply dummy text of the printing & type setting industry.',
    time:'11:59 am',
    img:user3,
    chat:true
  },
  {
    color: 'lightwarning',
    name: '',
    desc: 'Whats budget of the new project.',
    time:'10:59 am',
    img:user3,
    chat:false
  },
]

export { browserStatesData,recentChatsData }