import type { productPerformance } from '@/types/components/dashboards/EcommerceDash';

import type { EarningTypes } from '@/types/components/dashboards/EcommerceDash';
import user1 from '@/assets/images/profile/user-2.jpg';
import user2 from '@/assets/images/profile/user-2.jpg';
import user3 from '@/assets/images/profile/user-3.jpg';
import user4 from '@/assets/images/profile/user-4.jpg';
/*--Blog Cards--*/
const EarningData: EarningTypes[] = [
    {
        title: 'Pavan Kumar',
        avatar: user1,
        avatarstatus: 'success',
        desc: 'Web Designer',
        status: '$2300',
        color: 'primary'
    },
    {
        title: 'Sonu Nigam',
        avatar: user2,
        avatarstatus: 'error',
        desc: 'Web Developer',
        status: '$3300',
        color: 'secondary'
    },
    {
        title: 'Arijit singh',
        avatar: user3,
        avatarstatus: 'warning',
        desc: 'Web Manager',
        status: '$4300',
        color: 'error'
    },
    {
        title: 'Pavan Kumar',
        avatar: user4,
        avatarstatus: 'success',
        desc: 'Project Designer',
        status: '$4500',
        color: 'success'
    }
];


import img2 from '@/assets/images/products/chair2.png';
import img3 from '@/assets/images/products/chair3.png';
import img4 from '@/assets/images/products/chair4.png';

const productPerformanceData: productPerformance[] = [
    {
        avatar: img2,
        name: 'House Cleaner',
        qty: 25,
        date: '08-7-2024',
        status: 'Pending',
        statuscolor: 'warning'
    },
    {
        avatar: img3,
        name: 'Gray Chair',
        qty: 12,
        date: '05-7-2024',
        status: 'Paid',
        statuscolor: 'success'
    },
    {
        avatar: img4,
        name: 'New Gold Chair',
        qty: 18,
        date: '02-7-2024',
        status: 'Failed',
        statuscolor: 'error'
    }
];

export { productPerformanceData, EarningData };
