import { Helmet } from "react-helmet-async";

export default function SEO({
  title = "FlowerLab Vlada | Flowers Delivery Chernivtsi",
  description = "FlowerLab Vlada - Premium bouquets, fresh flowers, and gifts with delivery in Chernivtsi. Order online for best quality and service.",
  image = "/seo-image.jpg", // We should ideally add a default OG image to public folder
  url,
  type = "website",
}) {
  const siteUrl = "https://flowerlab-vlada.com"; // Replace with actual domain whenever available
  const currentUrl = url ? url : siteUrl;

  return (
    <Helmet>
      {/* Standard Metadata */}
      <title>{title}</title>
      <meta name="description" content={description} />
      <link rel="canonical" href={currentUrl} />

      {/* Open Graph / Facebook */}
      <meta property="og:type" content={type} />
      <meta property="og:url" content={currentUrl} />
      <meta property="og:title" content={title} />
      <meta property="og:description" content={description} />
      <meta property="og:image" content={image} />

      {/* Twitter */}
      <meta name="twitter:card" content="summary_large_image" />
      <meta name="twitter:url" content={currentUrl} />
      <meta name="twitter:title" content={title} />
      <meta name="twitter:description" content={description} />
      <meta name="twitter:image" content={image} />
    </Helmet>
  );
}
