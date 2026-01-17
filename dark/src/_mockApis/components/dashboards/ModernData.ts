import type { ProgressCards, ProductAvaibility } from '@/types/components/dashboards/ModernDash';

const ProgressCardsData: ProgressCards[] = [
    {
        percent: '86%',
        title: 'Total Product',
        progress: 86,
        progresscolor: 'primary'
    },
    {
        percent: '40%',
        title: 'Pending Product',
        progress: 40,
        progresscolor: 'secondary'
    },
    {
        percent: '56%',
        title: 'Product A',
        progress: 56,
        progresscolor: 'success'
    },
    {
        percent: '26%',
        title: 'Product B',
        progress: 26,
        progresscolor: 'error'
    }
];

const ProductAvaibilityData: ProductAvaibility[] = [
    {
        color: 'info',
        title: 'Apple iPhone 6 Space Grey, 16 GB',
        subtitle: 'Product id : MI5457',
        qty: 357,
        price: '$435'
    },
    {
        color: 'success',
        title: 'Fossil Marshall For Men Black watch',
        subtitle: 'Product id : MI5457',
        qty: 358,
        price: '$440'
    },
    {
        color: 'error',
        title: 'Sony Bravia 80cm - 32 HD Ready LED TV',
        subtitle: 'Product id : MI54578',
        qty: 300,
        price: '$500'
    },
    {
        color: 'primary',
        title: 'Panasonic P75 Champagne Gold, 8 GB',
        subtitle: 'Product id : MI54555',
        qty: 288,
        price: '$400'
    }
];
export { ProgressCardsData, ProductAvaibilityData };
