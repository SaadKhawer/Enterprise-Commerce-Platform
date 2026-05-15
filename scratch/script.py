import json

raw_data = """
**ELECTRONICS PRODUCTS (8 products):**

{
  id: 1,
  name: "iPhone 15 Pro Max",
  category: "Electronics",
  price: 450000,
  originalPrice: 540000,
  discount: 20,
  rating: 4.5,
  sold: 234,
  badge: "Hot",
  description: "Titanium design, A17 Pro chip, and most powerful iPhone camera ever.",
  image: "https://images.unsplash.com/photo-1695048133142-1a20484429be?w=400&h=400&fit=crop"
},
{
  id: 2,
  name: "Samsung Galaxy S24 Ultra",
  category: "Electronics",
  price: 285000,
  originalPrice: 342000,
  discount: 20,
  rating: 4.5,
  sold: 189,
  badge: "New",
  description: "Built-in S Pen, 200MP camera, AI-powered Galaxy experience.",
  image: "https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=400&h=400&fit=crop"
},
{
  id: 3,
  name: "MacBook Pro 14 inch",
  category: "Electronics",
  price: 650000,
  originalPrice: 780000,
  discount: 17,
  rating: 4.8,
  sold: 98,
  badge: "Hot",
  description: "M3 Pro chip, Liquid Retina XDR display, all-day battery life.",
  image: "https://images.unsplash.com/photo-1517336714731-489689fd9ca8?w=400&h=400&fit=crop"
},
{
  id: 4,
  name: "Sony WH-1000XM5 Headphones",
  category: "Electronics",
  price: 85000,
  originalPrice: 105000,
  discount: 19,
  rating: 4.7,
  sold: 312,
  badge: "Sale",
  description: "Industry-leading noise cancellation, 30-hour battery, premium sound.",
  image: "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&h=400&fit=crop"
},
{
  id: 5,
  name: "iPad Pro 12.9 inch",
  category: "Electronics",
  price: 380000,
  originalPrice: 450000,
  discount: 16,
  rating: 4.6,
  sold: 145,
  badge: "New",
  description: "M2 chip, Liquid Retina XDR, works with Apple Pencil and Magic Keyboard.",
  image: "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400&h=400&fit=crop"
},
{
  id: 6,
  name: "Samsung 55 inch 4K Smart TV",
  category: "Electronics",
  price: 195000,
  originalPrice: 234000,
  discount: 17,
  rating: 4.4,
  sold: 78,
  badge: "Sale",
  description: "Crystal UHD 4K, Smart TV with Alexa, HDR10+ support.",
  image: "https://images.unsplash.com/photo-1593359677879-a4bb92f829d1?w=400&h=400&fit=crop"
},
{
  id: 7,
  name: "Canon EOS R50 Camera",
  category: "Electronics",
  price: 220000,
  originalPrice: 265000,
  discount: 17,
  rating: 4.6,
  sold: 56,
  badge: "New",
  description: "24.2MP APS-C sensor, 4K video, perfect for content creators.",
  image: "https://images.unsplash.com/photo-1502920917128-1aa500764cbd?w=400&h=400&fit=crop"
},
{
  id: 8,
  name: "Dell 27 inch Gaming Monitor",
  category: "Electronics",
  price: 95000,
  originalPrice: 115000,
  discount: 17,
  rating: 4.5,
  sold: 167,
  badge: "Hot",
  description: "165Hz refresh rate, 1ms response time, IPS panel, QHD resolution.",
  image: "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=400&h=400&fit=crop"
},

**CLOTHING PRODUCTS (7 products):**

{
  id: 9,
  name: "Men's Premium Casual T-Shirt",
  category: "Clothing",
  price: 2500,
  originalPrice: 3500,
  discount: 29,
  rating: 4.3,
  sold: 523,
  badge: "Sale",
  description: "100% cotton, breathable fabric, available in multiple colors.",
  image: "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400&h=400&fit=crop"
},
{
  id: 10,
  name: "Women's Summer Floral Dress",
  category: "Clothing",
  price: 4500,
  originalPrice: 6000,
  discount: 25,
  rating: 4.5,
  sold: 289,
  badge: "Hot",
  description: "Light floral pattern, perfect for summer, available in S/M/L/XL.",
  image: "https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=400&h=400&fit=crop"
},
{
  id: 11,
  name: "Denim Jacket Unisex",
  category: "Clothing",
  price: 6500,
  originalPrice: 9000,
  discount: 28,
  rating: 4.4,
  sold: 178,
  badge: "Sale",
  description: "Classic denim jacket, slim fit, vintage wash finish.",
  image: "https://images.unsplash.com/photo-1544966503-7cc5ac882d5d?w=400&h=400&fit=crop"
},
{
  id: 12,
  name: "Running Sports Sneakers",
  category: "Clothing",
  price: 8500,
  originalPrice: 12000,
  discount: 29,
  rating: 4.6,
  sold: 445,
  badge: "Hot",
  description: "Lightweight, breathable mesh, cushioned sole for maximum comfort.",
  image: "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400&h=400&fit=crop"
},
{
  id: 13,
  name: "Women's Leather Handbag",
  category: "Clothing",
  price: 7500,
  originalPrice: 10000,
  discount: 25,
  rating: 4.4,
  sold: 234,
  badge: "New",
  description: "Genuine leather, multiple compartments, elegant design.",
  image: "https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=400&h=400&fit=crop"
},
{
  id: 14,
  name: "Men's Slim Fit Formal Shirt",
  category: "Clothing",
  price: 3200,
  originalPrice: 4500,
  discount: 29,
  rating: 4.3,
  sold: 367,
  badge: "Sale",
  description: "Premium cotton blend, wrinkle-resistant, perfect for office.",
  image: "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=400&h=400&fit=crop"
},
{
  id: 15,
  name: "Winter Puffer Jacket",
  category: "Clothing",
  price: 12000,
  originalPrice: 18000,
  discount: 33,
  rating: 4.7,
  sold: 156,
  badge: "Hot",
  description: "Warm insulation, water-resistant outer shell, hood included.",
  image: "https://images.unsplash.com/photo-1539533018447-63fcce2fae2e?w=400&h=400&fit=crop"
},

**BOOKS PRODUCTS (5 products):**

{
  id: 16,
  name: "Atomic Habits - James Clear",
  category: "Books",
  price: 1800,
  originalPrice: 2500,
  discount: 28,
  rating: 4.9,
  sold: 892,
  badge: "Hot",
  description: "Build good habits, break bad ones. #1 New York Times bestseller.",
  image: "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=400&h=400&fit=crop"
},
{
  id: 17,
  name: "Rich Dad Poor Dad",
  category: "Books",
  price: 1500,
  originalPrice: 2000,
  discount: 25,
  rating: 4.7,
  sold: 743,
  badge: "Hot",
  description: "What the rich teach their kids about money that the poor do not.",
  image: "https://images.unsplash.com/photo-1589829085413-56de8ae18c73?w=400&h=400&fit=crop"
},
{
  id: 18,
  name: "The Alchemist - Paulo Coelho",
  category: "Books",
  price: 1200,
  originalPrice: 1800,
  discount: 33,
  rating: 4.8,
  sold: 634,
  badge: "Sale",
  description: "A magical story about following your dreams and listening to your heart.",
  image: "https://images.unsplash.com/photo-1512820790803-83ca734da794?w=400&h=400&fit=crop"
},
{
  id: 19,
  name: "Python Programming for Beginners",
  category: "Books",
  price: 2200,
  originalPrice: 3000,
  discount: 27,
  rating: 4.5,
  sold: 412,
  badge: "New",
  description: "Complete guide to Python, includes projects and exercises.",
  image: "https://images.unsplash.com/photo-1515879218367-8466d910aaa4?w=400&h=400&fit=crop"
},
{
  id: 20,
  name: "The 7 Habits of Highly Effective People",
  category: "Books",
  price: 1600,
  originalPrice: 2200,
  discount: 27,
  rating: 4.7,
  sold: 567,
  badge: "Hot",
  description: "Powerful lessons in personal change by Stephen R. Covey.",
  image: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400&h=400&fit=crop"
},

**HOME & LIVING PRODUCTS (6 products):**

{
  id: 21,
  name: "Ceramic Coffee Mug Set of 6",
  category: "Home & Living",
  price: 3500,
  originalPrice: 5000,
  discount: 30,
  rating: 4.5,
  sold: 334,
  badge: "Sale",
  description: "Premium ceramic, dishwasher safe, beautiful minimalist design.",
  image: "https://images.unsplash.com/photo-1514228742587-6b1558fcca3d?w=400&h=400&fit=crop"
},
{
  id: 22,
  name: "LED Adjustable Desk Lamp",
  category: "Home & Living",
  price: 4500,
  originalPrice: 6500,
  discount: 31,
  rating: 4.4,
  sold: 223,
  badge: "New",
  description: "5 color modes, touch control, USB charging port, eye-care technology.",
  image: "https://images.unsplash.com/photo-1507473885765-e6ed9c4c7168?w=400&h=400&fit=crop"
},
{
  id: 23,
  name: "Smart Air Purifier",
  category: "Home & Living",
  price: 35000,
  originalPrice: 45000,
  discount: 22,
  rating: 4.6,
  sold: 89,
  badge: "Hot",
  description: "HEPA filter, removes 99.9% pollutants, WiFi enabled, covers 500 sq ft.",
  image: "https://images.unsplash.com/photo-1585771724684-38269d6639fd?w=400&h=400&fit=crop"
},
{
  id: 24,
  name: "Luxury Throw Pillow Set of 4",
  category: "Home & Living",
  price: 5500,
  originalPrice: 8000,
  discount: 31,
  rating: 4.3,
  sold: 178,
  badge: "Sale",
  description: "Soft velvet cover, hypoallergenic filling, elegant home decor.",
  image: "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=400&h=400&fit=crop"
},
{
  id: 25,
  name: "Stainless Steel Kitchen Knife Set",
  category: "Home & Living",
  price: 8500,
  originalPrice: 12000,
  discount: 29,
  rating: 4.7,
  sold: 267,
  badge: "Hot",
  description: "Professional grade, 8-piece set, ergonomic handles, ultra-sharp blades.",
  image: "https://images.unsplash.com/photo-1593618998160-e34014e67546?w=400&h=400&fit=crop"
},
{
  id: 26,
  name: "Aromatherapy Diffuser & Humidifier",
  category: "Home & Living",
  price: 6500,
  originalPrice: 9000,
  discount: 28,
  rating: 4.5,
  sold: 198,
  badge: "New",
  description: "Essential oil diffuser, 7 LED colors, auto shut-off, 500ml capacity.",
  image: "https://images.unsplash.com/photo-1602928321679-560bb453f190?w=400&h=400&fit=crop"
}
"""

import re
items = re.findall(r'\{(.*?)\}', raw_data, re.DOTALL)

cs_code = ""
img_code = ""
for item in items:
    name = re.search(r'name:\s*"(.*?)"', item).group(1)
    cat = re.search(r'category:\s*"(.*?)"', item).group(1)
    price = re.search(r'price:\s*(\d+)', item).group(1)
    rating = re.search(r'rating:\s*([\d\.]+)', item).group(1)
    sold = re.search(r'sold:\s*(\d+)', item).group(1)
    desc = re.search(r'description:\s*"(.*?)"', item).group(1)
    image = re.search(r'image:\s*"(.*?)"', item).group(1)
    
    catIdVar = ""
    if cat == "Electronics": catIdVar = "electronicsId"
    elif cat == "Clothing": catIdVar = "clothingId"
    elif cat == "Books": catIdVar = "booksId"
    elif cat == "Home & Living": catIdVar = "homeId"
    
    cs_code += f'new Product {{ ProductName = "{name}", Price = {price}, CategoryID = {catIdVar}, StockQuantity = 100, Description = "{desc}", Rating = {rating}m, TotalReviews = {sold} }},\n'
    img_code += f'if (nm == "{name.lower()}") return "{image}";\n'

with open("c:/Users/saads/Desktop/ShopWaveBlazor/scratch/gen.txt", "w") as f:
    f.write(cs_code + "\n\n" + img_code)
