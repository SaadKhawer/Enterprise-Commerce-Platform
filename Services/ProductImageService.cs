namespace ShopWaveBlazor.Web.Services;

public static class ProductImageService
{
    public static string GetImage(string? name, string? category)
    {
        var nm = name?.ToLower() ?? "";
        var cat = category?.ToLower() ?? "";

        if (nm == "iphone 15 pro max") return "https://images.unsplash.com/photo-1695048133142-1a20484429be?w=400&h=400&fit=crop";
        if (nm == "samsung galaxy s24 ultra") return "https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=400&h=400&fit=crop";
        if (nm == "macbook pro 14 inch") return "https://images.unsplash.com/photo-1517336714731-489689fd9ca8?w=400&h=400&fit=crop";
        if (nm == "sony wh-1000xm5 headphones") return "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&h=400&fit=crop";
        if (nm == "ipad pro 12.9 inch") return "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400&h=400&fit=crop";
        if (nm == "samsung 55 inch 4k smart tv") return "https://images.unsplash.com/photo-1593359677879-a4bb92f829d1?w=400&h=400&fit=crop";
        if (nm == "canon eos r50 camera") return "https://images.unsplash.com/photo-1502920917128-1aa500764cbd?w=400&h=400&fit=crop";
        if (nm == "dell 27 inch gaming monitor") return "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=400&h=400&fit=crop";
        if (nm == "men's premium casual t-shirt") return "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400&h=400&fit=crop";
        if (nm == "women's summer floral dress") return "https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=400&h=400&fit=crop";
        if (nm == "denim jacket unisex") return "https://images.unsplash.com/photo-1544966503-7cc5ac882d5d?w=400&h=400&fit=crop";
        if (nm == "running sports sneakers") return "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400&h=400&fit=crop";
        if (nm == "women's leather handbag") return "https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=400&h=400&fit=crop";
        if (nm == "men's slim fit formal shirt") return "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=400&h=400&fit=crop";
        if (nm == "winter puffer jacket") return "https://images.unsplash.com/photo-1539533018447-63fcce2fae2e?w=400&h=400&fit=crop";
        if (nm == "atomic habits - james clear") return "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=400&h=400&fit=crop";
        if (nm == "rich dad poor dad") return "https://images.unsplash.com/photo-1589829085413-56de8ae18c73?w=400&h=400&fit=crop";
        if (nm == "the alchemist - paulo coelho") return "https://images.unsplash.com/photo-1512820790803-83ca734da794?w=400&h=400&fit=crop";
        if (nm == "python programming for beginners") return "https://images.unsplash.com/photo-1515879218367-8466d910aaa4?w=400&h=400&fit=crop";
        if (nm == "the 7 habits of highly effective people") return "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400&h=400&fit=crop";
        if (nm == "ceramic coffee mug set of 6") return "https://images.unsplash.com/photo-1514228742587-6b1558fcca3d?w=400&h=400&fit=crop";
        if (nm == "led adjustable desk lamp") return "https://images.unsplash.com/photo-1507473885765-e6ed9c4c7168?w=400&h=400&fit=crop";
        if (nm == "smart air purifier") return "https://images.unsplash.com/photo-1585771724684-38269d6639fd?w=400&h=400&fit=crop";
        if (nm == "luxury throw pillow set of 4") return "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=400&h=400&fit=crop";
        if (nm == "stainless steel kitchen knife set") return "https://images.unsplash.com/photo-1593618998160-e34014e67546?w=400&h=400&fit=crop";
        if (nm == "aromatherapy diffuser & humidifier") return "https://images.unsplash.com/photo-1602928321679-560bb453f190?w=400&h=400&fit=crop";

        // FALLBACKS BY CATEGORY
        if (cat.Contains("electronic")) return "https://images.unsplash.com/photo-1498049794561-7780e7231661?w=600&q=80";
        if (cat.Contains("cloth") || cat.Contains("fashion")) return "https://images.unsplash.com/photo-1445205170230-053b83016050?w=600&q=80";
        if (cat.Contains("book")) return "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?w=600&q=80";
        if (cat.Contains("home") || cat.Contains("kitchen") || cat.Contains("living")) return "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=600&q=80";

        return "https://images.unsplash.com/photo-1560472355-536de3962603?w=600&q=80";
    }
}
