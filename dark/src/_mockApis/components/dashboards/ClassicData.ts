/*import tabler icons*/
import { CalendarIcon, CreditCardIcon, SettingsIcon, UsersIcon } from 'vue-tabler-icons';
import type { revenueCards, projectTable, recentComments,todoList } from '@/types/components/dashboards/ClassicDash';
import img1 from '@/assets/images/profile/user-2.jpg';
import img2 from '@/assets/images/profile/user-2.jpg';
import img3 from '@/assets/images/profile/user-3.jpg';
import img4 from '@/assets/images/profile/user-4.jpg';
import img5 from '@/assets/images/profile/user-5.jpg';

const revenueCardsData: revenueCards[] = [
    {
        title: "Total Revenue",
        iconcolor: "bg-primary",
        icon: CreditCardIcon,
        number: "$3249",
    },
    {
        title: "Online Revenue",
        iconcolor: "bg-secondary",
        icon: UsersIcon,
        number: "$2376",
    },
    {
        title: "Offline Revenue",
        iconcolor: "bg-error",
        icon: CalendarIcon,
        number: "$1795",
    },
    {
        title: "Ad. Expense",
        iconcolor: "bg-warning",
        icon: SettingsIcon,
        number: "$687",
    },
];

const projectTableData: projectTable[] = [
    {
        img: img1,
        activestate: "",
        leadname: "Sunil Joshi",
        leademail: "Web Designer",
        projectname: "Elite Admin",
        statuscolor: "primary",
        statustext: "Low",
        money: "$3.9K",
    },
    {
        img: img2,
        activestate: "active",
        leadname: "Andrew",
        leademail: "Project Manager",
        projectname: "Real Homes",
        statuscolor: "info",
        statustext: "Medium",
        money: "$23.9K",
    },
    {
        img: img3,
        activestate: "",
        leadname: "Bhavesh patel",
        leademail: "Developer",
        projectname: "MedicalPro Theme",
        statuscolor: "success",
        statustext: "High",
        money: "$12.9K",
    },
    {
        img: img4,
        activestate: "",
        leadname: "Nirav Joshi",
        leademail: "Frontend Eng",
        projectname: "Elite Admin",
        statuscolor: "error",
        statustext: "Low",
        money: "$10.9K",
    },
   
];

const recentCommentsData: recentComments[] = [
    {
        img: img1,
        activeClass: "",
        username: "James Anderson",
        desc: "Lorem Ipsum is simply dummy text of the printing and type setting industry.",
        date: "April 14, 2022",
        badgebg: "warning",
        status: "Pending",
    },
    {
        img: img2,
        activeClass: "active",
        username: "Michael Jorden",
        desc: "Lorem Ipsum has beenorem Ipsum is simply dummy text of the printing and type setting industry.",
        date: "April 14, 2022",
        badgebg: "success",
        status: "Approved",
    },
    {
        img: img3,
        activeClass: "",
        username: "Johnathan Doeting",
        desc: "Lorem Ipsum is simply dummy text of the printing and type setting industry.",
        date: "April 14, 2022",
        badgebg: "error",
        status: "Rejected",
    },
   
];


import todo1 from '@/assets/images/blog/blog-img2.jpg';
import todo2 from '@/assets/images/blog/blog-img3.jpg';
import todo3 from '@/assets/images/blog/blog-img4.jpg';
import todo4 from '@/assets/images/blog/blog-img5.jpg';

const todoListData: todoList[] = [
    {
        img: todo1,
        title: "Schedule meeting with",
        desc: "Phasellus quis rutrum leo quis vulputate tortor Lorem Ipsum is simply dummy text of the printing and type setting industry.",
        badgebg: "primary",
        status: "Today",
    },
    {
        img: todo2,
        title: "Forward all tasks",
        desc: "Mauris cursus scelerisque felis et ultricies Lorem Ipsum is simply dummy text of the printing and type setting industry.",
        badgebg: "success",
        status: "Yesterday",
    },
    {
        img: todo3,
        title: "Give Purchase report to",
        desc: "Vestibulum non aliquet mi vitae mollis lorem Ipsum is simply dummy text of the printing and type setting industry.",
        badgebg: "error",
        status: "Tomorrow",
    },
    {
        img: todo4,
        title: "Book flight for holiday",
        desc: "Aenean interdum auctor massa ut scelerisque lorem Ipsum is simply dummy text of the printing and type setting industry.",
        badgebg: "warning",
        status: "1 Week",
    },
]


export { revenueCardsData, projectTableData, recentCommentsData,todoListData };
