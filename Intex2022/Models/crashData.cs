using System;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace UDOT.Models
{
    public class crashData
    {
        public float milepoint { get; set; }
        public float pedestrian_involved { get; set; }
        public float dui { get; set; }
        public float intersection_related { get; set; }
        public float wild_animal_related { get; set; }
        public float overturn_rollover { get; set; }
        public float teenage_driver_involved { get; set; }
        public float older_driver_involved { get; set; }
        public float single_vehicle { get; set; }
        public float distracted_driving { get; set; }
        public float roadway_departure { get; set; }
        public float city_OUTSIDE_CITY_LIMITS { get; set; }
        public float city_Other { get; set; }
        public float city_SANDY { get; set; }
        public float county_name_SALT_LAKE { get; set; }
        public float crash_severity_id { get; set; }


        public Tensor<float> AsTensor()
        {
            float[] data = new float[]
            {
                milepoint, pedestrian_involved, dui, intersection_related, wild_animal_related, overturn_rollover,
                teenage_driver_involved, older_driver_involved, single_vehicle, distracted_driving, roadway_departure,
                city_OUTSIDE_CITY_LIMITS, city_Other,  city_SANDY,  county_name_SALT_LAKE, crash_severity_id
            };
            int[] dimensions = new int[] { 1, 16 };
            return new DenseTensor<float>(data, dimensions);
        }
    }
}
