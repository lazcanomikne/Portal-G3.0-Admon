import type { myContacts } from '@/types/components/dashboards/AnalyticalDash';
import img1 from '@/assets/images/profile/user-2.jpg';
import img2 from '@/assets/images/profile/user-2.jpg';
import img3 from '@/assets/images/profile/user-3.jpg';
import img4 from '@/assets/images/profile/user-4.jpg';
import img5 from '@/assets/images/profile/user-5.jpg';
/*--Blog Cards--*/
const myContactsData: myContacts[] = [
    {
        title: "Pavan Kumar",
        avatar: img1,
        avatarstatus: "success",
        desc: "you were in video call",
      },
      {
        title: "Sonu Nigam",
        avatar: img2,
        avatarstatus: "error",
        desc: "you were in video call",
      },
      {
        title: "Arijit singh",
        avatar: img3,
        avatarstatus: "warning",
        desc: "you missed john call",
      },
      {
        title: "Pavan Kumar",
        avatar: img4,
        avatarstatus: "success",
        desc: "you were in video call",
      },
      {
        title: "Mark Daw",
        avatar: img5,
        avatarstatus: "primary",
        desc: "you were in video call",
      },
];

export { myContactsData }