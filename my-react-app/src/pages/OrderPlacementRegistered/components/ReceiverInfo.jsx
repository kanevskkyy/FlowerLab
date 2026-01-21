import React from "react";
import { useFormContext } from "react-hook-form";

const ReceiverInfo = () => {
  const {
    register,
    watch,
    formState: { errors },
  } = useFormContext();

  const receiverType = watch("receiverType");

  return (
    <section className="form-section receiver-section-wrapper">
      <div className="receiver-section">
        <label>Receiver:</label>
        <div className="radio-group">
          <label className="radio-label">
            <input type="radio" value="self" {...register("receiverType")} />
            <span>I am the receiver</span>
          </label>
          <label className="radio-label">
            <input type="radio" value="other" {...register("receiverType")} />
            <span>The receiver is other person</span>
          </label>
        </div>
      </div>

      {/* Conditional Receiver Fields */}
      {receiverType === "other" && (
        <div className="receiver-fields fade-in-section">
          <div className="form-group">
            <label>Receiver Name</label>
            <input
              type="text"
              placeholder="Receiver Name"
              style={
                errors.receiverName
                  ? {
                      border: "1px solid #d32f2f",
                    }
                  : {}
              }
              {...register("receiverName")}
            />
            {errors.receiverName && (
              <p className="error-text">{errors.receiverName.message}</p>
            )}
          </div>
          <div className="form-group">
            <label>Receiver Phone</label>
            <input
              type="tel"
              placeholder="Receiver Phone"
              style={
                errors.receiverPhone
                  ? {
                      border: "1px solid #d32f2f",
                    }
                  : {}
              }
              {...register("receiverPhone")}
            />
            {errors.receiverPhone && (
              <p className="error-text">{errors.receiverPhone.message}</p>
            )}
          </div>
        </div>
      )}
    </section>
  );
};

export default ReceiverInfo;
