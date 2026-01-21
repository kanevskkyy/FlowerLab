import React from "react";
import { useFormContext } from "react-hook-form";

const ContactInfo = () => {
  const {
    register,
    formState: { errors },
  } = useFormContext();

  return (
    <section className="form-section">
      <h2>Your contact information</h2>

      <div className="form-group">
        <label>First Name</label>
        <input
          type="text"
          placeholder="First Name"
          style={errors.firstName ? { border: "1px solid #d32f2f" } : {}}
          {...register("firstName")}
        />
        {errors.firstName && (
          <p className="error-text">{errors.firstName.message}</p>
        )}
      </div>

      <div className="form-group">
        <label>Last Name</label>
        <input
          type="text"
          placeholder="Last Name"
          style={errors.lastName ? { border: "1px solid #d32f2f" } : {}}
          {...register("lastName")}
        />
        {errors.lastName && (
          <p className="error-text">{errors.lastName.message}</p>
        )}
      </div>

      <div className="form-group">
        <label>Phone</label>
        <input
          type="tel"
          placeholder="+38 066 001 02 03"
          style={errors.phone ? { border: "1px solid #d32f2f" } : {}}
          {...register("phone")}
        />
        {errors.phone && <p className="error-text">{errors.phone.message}</p>}
      </div>
    </section>
  );
};

export default ContactInfo;
