// src/data/productsData.js
// Імпортуйте всі потрібні зображення у Catalog.jsx та ProductCard.jsx

export const productsData = [
  {
    id: 1,
    title: "Spring Sunrise",
    price: 1000,
    mainImage: "bouquet1L", // використовуйте це як ключ для імпорту
    images: {
      S: "bouquet1S",
      M: "bouquet1M",
      L: "bouquet1L",
      XL: "bouquet1XL"
    },
    prices: {
      S: 800,
      M: 1000,
      L: 1200,
      XL: 1500
    },
    description: "A delicate bouquet combining soft pastel tones and natural textures. Perfect for any occasion — from friendly greetings to special celebrations.",
    composition: "orchids, peonies, greenery",
    size: "M",
    quantity: 51,
    events: ["Birthday", "Anniversary"],
    forWho: ["Mom", "Wife"],
    flowerType: ["Orchid", "Peony"],
    popularity: 95,
    dateAdded: "2024-12-10"
  },
  {
    id: 2,
    title: "Royal Romance",
    price: 2000,
    mainImage: "bouquet2L",
    images: {
      S: "bouquet2S",
      M: "bouquet2M",
      L: "bouquet2L",
      XL: "bouquet2XL"
    },
    prices: {
      S: 1200,
      M: 1800,
      L: 2300,
      XL: 3000
    },
    description: "Elegant arrangement with premium roses and seasonal flowers. A luxurious gift for someone special.",
    composition: "roses, tulips, eucalyptus",
    size: "L",
    quantity: 101,
    events: ["Wedding", "Anniversary", "Engagement"],
    forWho: ["Wife", "Husband"],
    flowerType: ["Rose", "Tulip"],
    popularity: 98,
    dateAdded: "2024-12-08"
  },
  {
    id: 3,
    title: "Garden Dreams",
    price: 1800,
    mainImage: "bouquet3L",
    images: {
      S: "bouquet3S",
      M: "bouquet3M",
      L: "bouquet3L",
      XL: "bouquet3XL"
    },
    prices: {
      S: 700,
      M: 900,
      L: 1100,
      XL: 1300
    },
    description: "Charming spring bouquet with bright colors. Fresh and vibrant, perfect for bringing joy.",
    composition: "tulips, eustomas, greenery",
    size: "M",
    quantity: 51,
    events: ["Birthday"],
    forWho: ["Mom", "Teacher", "Co-worker"],
    flowerType: ["Tulip"],
    popularity: 87,
    dateAdded: "2024-12-11"
  },
  {
    id: 4,
    title: "Sunset Blush",
    price: 1500,
    mainImage: "bouquet4L",
    images: {
      S: "bouquet4S",
      M: "bouquet4M",
      L: "bouquet4L",
      XL: "bouquet4XL"
    },
    prices: {
      S: 900,
      M: 1500,
      L: 1900,
      XL: 2500
    },
    description: "Warm sunset tones with premium roses and delicate accents. Perfect for romantic occasions.",
    composition: "roses, carnations, baby's breath",
    size: "L",
    quantity: 101,
    events: ["Anniversary", "Engagement"],
    forWho: ["Wife", "Mom"],
    flowerType: ["Rose", "Carnation"],
    popularity: 92,
    dateAdded: "2024-12-09"
  },
  {
    id: 5,
    title: "Lavender Elegance",
    price: 1300,
    mainImage: "bouquet5L",
    images: {
      S: "bouquet5S",
      M: "bouquet5M",
      L: "bouquet5L",
      XL: "bouquet5XL"
    },
    prices: {
      S: 800,
      M: 1300,
      L: 1700,
      XL: 2200
    },
    description: "Soft lavender hues combined with white accents. Elegant and sophisticated arrangement.",
    composition: "hydrangeas, roses, eucalyptus",
    size: "M",
    quantity: 51,
    events: ["Wedding", "Birthday"],
    forWho: ["Mom", "Wife", "Teacher"],
    flowerType: ["Hydrangea", "Rose"],
    popularity: 89,
    dateAdded: "2024-12-07"
  },
  {
    id: 6,
    title: "Tropical Paradise",
    price: 2500,
    mainImage: "bouquet6L",
    images: {
      S: "bouquet6S",
      M: "bouquet6M",
      L: "bouquet6L",
      XL: "bouquet6XL"
    },
    prices: {
      S: 1500,
      M: 2500,
      L: 3200,
      XL: 4000
    },
    description: "Exotic tropical flowers in vibrant colors. A statement piece for special celebrations.",
    composition: "orchids, birds of paradise, tropical greenery",
    size: "XL",
    quantity: 201,
    events: ["Wedding", "Anniversary"],
    forWho: ["Wife", "Husband"],
    flowerType: ["Orchid"],
    popularity: 94,
    dateAdded: "2024-12-06"
  },
  {
    id: 7,
    title: "Classic White",
    price: 1700,
    mainImage: "bouquet7L",
    images: {
      S: "bouquet7S",
      M: "bouquet7M",
      L: "bouquet7L",
      XL: "bouquet7XL"
    },
    prices: {
      S: 1000,
      M: 1700,
      L: 2200,
      XL: 2800
    },
    description: "Pure white arrangement with elegant lilies and roses. Timeless and sophisticated.",
    composition: "lilies, roses, baby's breath",
    size: "L",
    quantity: 101,
    events: ["Wedding", "Anniversary"],
    forWho: ["Wife", "Mom"],
    flowerType: ["Lily", "Rose"],
    popularity: 96,
    dateAdded: "2024-12-05"
  },
  {
    id: 8,
    title: "Sunshine Joy",
    price: 1200,
    mainImage: "bouquet8L",
    images: {
      S: "bouquet8S",
      M: "bouquet8M",
      L: "bouquet8L",
      XL: "bouquet8XL"
    },
    prices: {
      S: 700,
      M: 1200,
      L: 1600,
      XL: 2000
    },
    description: "Bright yellow blooms that bring happiness and warmth. Perfect for cheerful occasions.",
    composition: "sunflowers, daisies, greenery",
    size: "M",
    quantity: 51,
    events: ["Birthday"],
    forWho: ["Mom", "Kid", "Co-worker"],
    flowerType: ["Chrysanthemum", "Daffodil"],
    popularity: 85,
    dateAdded: "2024-12-12"
  },
  {
    id: 9,
    title: "Pink Perfection",
    price: 1400,
    mainImage: "bouquet9L",
    images: {
      S: "bouquet9S",
      M: "bouquet9M",
      L: "bouquet9L",
      XL: "bouquet9XL"
    },
    prices: {
      S: 850,
      M: 1400,
      L: 1800,
      XL: 2300
    },
    description: "Soft pink tones with premium peonies. Romantic and delicate arrangement.",
    composition: "peonies, roses, ranunculus",
    size: "M",
    quantity: 101,
    events: ["Birthday", "Anniversary"],
    forWho: ["Mom", "Wife"],
    flowerType: ["Peony", "Rose"],
    popularity: 91,
    dateAdded: "2024-12-11"
  },
  {
    id: 10,
    title: "Autumn Harmony",
    price: 1600,
    mainImage: "bouquet10L",
    images: {
      S: "bouquet10S",
      M: "bouquet10M",
      L: "bouquet10L",
      XL: "bouquet10XL"
    },
    prices: {
      S: 950,
      M: 1600,
      L: 2000,
      XL: 2600
    },
    description: "Warm autumn colors with rich textures. Perfect for fall celebrations.",
    composition: "chrysanthemums, roses, autumn foliage",
    size: "L",
    quantity: 101,
    events: ["Birthday", "Anniversary"],
    forWho: ["Mom", "Teacher", "Co-worker"],
    flowerType: ["Chrysanthemum", "Rose"],
    popularity: 88,
    dateAdded: "2024-12-04"
  },
  {
    id: 11,
    title: "Purple Majesty",
    price: 1900,
    mainImage: "bouquet11XL",
    images: {
      S: "bouquet11S",
      M: "bouquet11M",
      L: "bouquet11L",
      XL: "bouquet11XL"
    },
    prices: {
      S: 1100,
      M: 1900,
      L: 2400,
      XL: 3100
    },
    description: "Rich purple hues with elegant orchids. Luxurious and eye-catching.",
    composition: "orchids, lisianthus, purple stock",
    size: "L",
    quantity: 101,
    events: ["Wedding", "Anniversary"],
    forWho: ["Wife", "Husband"],
    flowerType: ["Orchid"],
    popularity: 93,
    dateAdded: "2024-12-03"
  },
  {
    id: 12,
    title: "Spring Meadow",
    price: 1100,
    mainImage: "bouquet12L",
    images: {
      S: "bouquet12S",
      M: "bouquet12M",
      L: "bouquet12L",
      XL: "bouquet12XL"
    },
    prices: {
      S: 650,
      M: 1100,
      L: 1500,
      XL: 1900
    },
    description: "Fresh spring flowers in a natural arrangement. Light and airy design.",
    composition: "tulips, daffodils, wildflowers",
    size: "S",
    quantity: 51,
    events: ["Birthday"],
    forWho: ["Mom", "Kid", "Teacher"],
    flowerType: ["Tulip", "Daffodil"],
    popularity: 82,
    dateAdded: "2024-12-02"
  },
  {
    id: 13,
    title: "Red Passion",
    price: 2200,
    mainImage: "bouquet13L",
    images: {
      S: "bouquet13S",
      M: "bouquet13M",
      L: "bouquet13L",
      XL: "bouquet13XL"
    },
    prices: {
      S: 1300,
      M: 2200,
      L: 2800,
      XL: 3500
    },
    description: "Bold red roses in a dramatic arrangement. Perfect for expressing deep emotions.",
    composition: "red roses, hypericum, greenery",
    size: "XL",
    quantity: 201,
    events: ["Anniversary", "Engagement"],
    forWho: ["Wife", "Husband"],
    flowerType: ["Rose"],
    popularity: 99,
    dateAdded: "2024-12-01"
  },
  {
    id: 14,
    title: "Gentle Touch",
    price: 1250,
    mainImage: "bouquet14XL",
    images: {
      S: "bouquet14S",
      M: "bouquet14M",
      L: "bouquet14L",
      XL: "bouquet14XL"
    },
    prices: {
      S: 750,
      M: 1250,
      L: 1650,
      XL: 2100
    },
    description: "Soft pastel arrangement with delicate touches. Sweet and charming design.",
    composition: "carnations, spray roses, baby's breath",
    size: "M",
    quantity: 51,
    events: ["Birthday"],
    forWho: ["Mom", "Kid", "Teacher", "Co-worker"],
    flowerType: ["Carnation", "Rose"],
    popularity: 84,
    dateAdded: "2024-11-30"
  },
  {
    id: 15,
    title: "Ocean Breeze",
    price: 1800,
    mainImage: "bouquet15L",
    images: {
      S: "bouquet15S",
      M: "bouquet15M",
      L: "bouquet15L",
      XL: "bouquet15XL"
    },
    prices: {
      S: 1050,
      M: 1800,
      L: 2300,
      XL: 2900
    },
    description: "Cool blue and white tones with a fresh feel. Calming and elegant.",
    composition: "hydrangeas, delphiniums, white roses",
    size: "L",
    quantity: 101,
    events: ["Wedding", "Anniversary"],
    forWho: ["Wife", "Mom", "Husband"],
    flowerType: ["Hydrangea", "Rose"],
    popularity: 90,
    dateAdded: "2024-11-29"
  }
];